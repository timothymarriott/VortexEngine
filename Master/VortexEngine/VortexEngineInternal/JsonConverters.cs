using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace VortexEngine;

public class ComponentConverter : JsonConverter<Component>
{

    public class Patch
    {
        public Component comp;
        public string field;
        public string targetBody;
    }
    
    public static List<Patch> patches = new List<Patch>();

    public void Deserialize(ref Utf8JsonReader reader)
    {
        
    }
    
    public static Component ReadActualComponent(ref Utf8JsonReader reader, JsonSerializerOptions options){
        if (reader.TokenType != JsonTokenType.StartObject)
        {
            throw new JsonException("Expected StartObject token");
        }
        reader.Read();

        if (reader.TokenType != JsonTokenType.PropertyName || reader.GetString() != "type")
        {
            throw new JsonException("Expected 'type' property");
        }
        reader.Read();

        var typeInfo = JsonSerializer.Deserialize<ComponentTypeInfo>(ref reader, options);
        
        reader.Read();
        if (reader.TokenType != JsonTokenType.PropertyName || reader.GetString() != typeInfo.Name)
        {
            throw new JsonException($"Expected '{typeInfo.Name}' as the component type key");
           
            
        }
        reader.Read();

       
        var componentType = Type.GetType($"{typeInfo.Fullname}, {typeInfo.Assembly}");
        if (typeInfo.Assembly == VortexEngine.ProjectAssembly.FullName.Split(',')[0] && componentType == null){
            componentType = VortexEngine.ProjectAssembly.GetType(typeInfo.Fullname);
        }

        if (componentType == null)
        {
            throw new JsonException($"Could not load type: {typeInfo.Fullname} from assembly: {typeInfo.Assembly}");
        }


        var newoptions = new JsonSerializerOptions
        {
            IncludeFields = true,
            Converters = { new BodyConverter(), new ComponentConverter() },
            ReferenceHandler = ReferenceHandler.Preserve
        };
        
        Component component = (Component)Activator.CreateInstance(componentType);

        reader.Read();
        while (reader.TokenType == JsonTokenType.PropertyName)
        {
            
            FieldInfo fieldInfo = componentType.GetField(reader.GetString());
            if (fieldInfo == null)
            {
                string name = reader.GetString();
                PropertyInfo propertyInfo = componentType.GetProperty(name);
                if (propertyInfo == null)
                {
                    JsonSerializer.Deserialize(ref reader, typeof(object), newoptions);
                }
                else
                {
                    if (propertyInfo.PropertyType.IsAssignableTo(typeof(Component)))
                    {
                        QueuedComponent queuedComp = (QueuedComponent)JsonSerializer.Deserialize(ref reader, typeof(QueuedComponent), newoptions);
                        patches.Add(new Patch()
                        {
                            comp = component,
                            field = propertyInfo.Name,
                            targetBody = queuedComp.body,
                        });
                    }
                    else
                    {
                        propertyInfo.SetValue(component, JsonSerializer.Deserialize(ref reader, propertyInfo.PropertyType, newoptions));
                    }
                }
                
            }
            else
            {
                if (fieldInfo.FieldType.IsAssignableTo(typeof(Component)))
                {
                    QueuedComponent queuedComp = (QueuedComponent)JsonSerializer.Deserialize(ref reader, typeof(QueuedComponent), newoptions);
                    patches.Add(new Patch()
                    {
                        comp = component,
                        field = fieldInfo.Name,
                        targetBody = queuedComp.body,
                    });
                }
                else
                {
                    fieldInfo.SetValue(component, JsonSerializer.Deserialize(ref reader, fieldInfo.FieldType, newoptions));
                }
            }
            
            
            
            

            reader.Read();
        }

        reader.Read();
        
        return component;
    }

    public override Component Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return ReadActualComponent(ref reader, options);
    }


    public static void WriteActualComponent(ref Utf8JsonWriter writer, Component value, JsonSerializerOptions options){
        writer.WriteStartObject();

        // Write the "type" field with type information
        var typeInfo = new ComponentTypeInfo
        {
            Name = value.GetType().Name,
            Fullname = value.GetType().FullName,
            Assembly = value.GetType().Assembly.FullName.Split(',')[0] // Only assembly name
        };
        writer.WritePropertyName("type");
        JsonSerializer.Serialize(writer, typeInfo, options);



        writer.WritePropertyName(value.GetType().Name);

        writer.WriteStartObject();


        foreach (var field in value.GetType().GetFields())
        {

            //field.Name != nameof(Component.body) && field.Name != nameof(Component.transform) && field.Name != "physicsBody"
            if (field.GetCustomAttribute<JsonIgnoreAttribute>() == null){
                writer.WritePropertyName(field.Name);
                
                if (field.FieldType.IsSubclassOf(typeof(Component))){
                    
                    var _value = field.GetValue(value) as Component;

                    if (_value != null ? (_value.body != null) : false){
                        writer.WriteStartObject();
                        
                        //options.ReferenceHandler = ReferenceHandler.Preserve;

                        // Write the "type" field with type information
                        var _typeInfo = new ComponentTypeInfo
                        {
                            Name = _value.GetType().Name,
                            Fullname = _value.GetType().FullName,
                            Assembly = _value.GetType().Assembly.FullName.Split(',')[0] // Only assembly name
                        };
                        writer.WritePropertyName("type");
                        JsonSerializer.Serialize(writer, _typeInfo, options);

                        writer.WriteString("body", _value.body.ID);

                        writer.WriteEndObject();
                    } else {
                        writer.WriteNullValue();
                    }


                    /*
                    JsonSerializer.Serialize(writer, field.GetValue(value), new JsonSerializerOptions
                    {
                        IncludeFields = true,               // Include fields in deserialization
                        Converters = { new BodyConverter() }
                    });*/
                } else {
                    
                    JsonSerializer.Serialize(writer, field.GetValue(value), new JsonSerializerOptions
                    {
                        IncludeFields = true,               // Include fields in deserialization
                        Converters = { new BodyConverter() },
                        ReferenceHandler = ReferenceHandler.Preserve
                    });
                }
                
                

            }
        }
        
        foreach (var field in value.GetType().GetProperties())
        {

            //field.Name != nameof(Component.body) && field.Name != nameof(Component.transform) && field.Name != "physicsBody"
            if (field.GetCustomAttribute<JsonIgnoreAttribute>() == null){
                writer.WritePropertyName(field.Name);
            
                JsonSerializer.Serialize(writer, field.GetValue(value), new JsonSerializerOptions
                {
                    IncludeFields = true,               // Include fields in deserialization
                    Converters = { new BodyConverter() },
                    ReferenceHandler = ReferenceHandler.Preserve

                });
                

            }
        }
        

        writer.WriteEndObject();


        writer.WriteEndObject();
    }

    public override void Write(Utf8JsonWriter writer, Component value, JsonSerializerOptions options)
    {

        WriteActualComponent(ref writer, value, options);
    }

    
}


[Serializable]
public class ComponentReference {
    [JsonPropertyName("type")]
    public ComponentTypeInfo type { get; set; }

    [JsonPropertyName("body")]
    public string body { get; set; }
}

[Serializable]
public class ComponentTypeInfo
{
    [JsonPropertyName("Name")]
    public string Name { get; set; } = "";

    [JsonPropertyName("Fullname")]
    public string Fullname { get; set; } = "";

    [JsonPropertyName("Assembly")]
    public string Assembly { get; set; } = "";
}

public class SceneConverter : JsonConverter<Scene>
{

    public override Scene Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {

        options = new JsonSerializerOptions
        {
            IncludeFields = true,
            WriteIndented = true,
            Converters = { new ComponentConverter(), new SceneConverter() },
            ReferenceHandler = ReferenceHandler.Preserve
        };

        if (reader.TokenType != JsonTokenType.StartObject)
        {
            throw new JsonException("Expected StartObject token");
        }

        Scene scene = new Scene();

        reader.Read();

        while (reader.TokenType == JsonTokenType.PropertyName)
        {
            string propertyName = reader.GetString();
            reader.Read();

            if (propertyName == "Bodies")
            {
                scene.Bodys = JsonSerializer.Deserialize<List<Body>>(ref reader, options);
            }
            else
            {
                throw new JsonException($"Unknown property: {propertyName}");
            }

            reader.Read();
        }

        return scene;
    }

    public override void Write(Utf8JsonWriter writer, Scene value, JsonSerializerOptions options)
    {
        options = new JsonSerializerOptions
        {
            IncludeFields = true,
            WriteIndented = true,
            Converters = { new ComponentConverter(), new SceneConverter() },
            ReferenceHandler = ReferenceHandler.Preserve
        };
        writer.WriteStartObject();

        writer.WritePropertyName("Bodies");

        JsonSerializer.Serialize(writer, value.Bodys, typeof(List<Body>), options);

        writer.WriteEndObject();
    }
}


public class BodyConverter : JsonConverter<Body>
{
    
    public override Body? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.String)
        {
            reader.Read();
            return null;
        }
        QueuedBody body = new QueuedBody(reader.GetString());
        return body;
    }

    public override void Write(Utf8JsonWriter writer, Body value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ID);
    }
}
