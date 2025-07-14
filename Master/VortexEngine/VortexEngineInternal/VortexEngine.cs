using VortexEngine.Rendering;
using System.Text.Json;
using System.Reflection;
using System.Text.Json.Serialization;
using VortexEngine.Internal.AssetManagement;

namespace VortexEngine;



[Serializable]
public class ProjectSettings
{
    public byte[] icon;
    public string ProjectName;
    public string DefaultScene = "scene.vscn";
}

public abstract class VortexEngine
{


    public static float UNIT_SCALE = 25;


    public static VortexEngine Master;

    public Scene LoadedScene = new Scene();

    public Dictionary<string, InvokeMethodData> InvokeQueue = new Dictionary<string, InvokeMethodData>();

    Texture? frameTexture;

    public bool DrawGizmos = false;
    #if DEBUG
    public static string EditorDataPath = "../BuildData/";
    #else
    public static string EditorDataPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
    #endif

    public static bool InEditor = false;
    
    public ProjectSettings ProjectSettings;
    
    public static string ProjectPath;
    public static string ProjectDataPath {
        get {
            if (Environment.OSVersion.Platform == PlatformID.Win32NT){
                return ProjectPath + "Assets\\";
            }
            return ProjectPath + "Assets/";

        }
        set {
            return;
        }
    }

    public abstract void Start();

    public static Assembly ProjectAssembly;

    public VortexEngine (){
        Master = this;
    }

    public void Init()
    {

        Master = this;
        
        #if !DEBUG
        //AppDomain.CurrentDomain.UnhandledException += UnhandledExceptionHandler;
        #endif
        
        LoadedScene.Start();

    }

    private static void UnhandledExceptionHandler(object sender, UnhandledExceptionEventArgs e)
    {
        Exception ex = (Exception)e.ExceptionObject;
        Log.ShowStackTrace = false;
        Log.Fatal($"Unhandled exception of type {ex.GetType().Name}\n{ex.Message}\n{ex.StackTrace}");
        Log.ShowStackTrace = true;
       
    }

    public Body InstantiateBodyWith<T>() where T : Component, new()
    {
        Body body = new Body();
        body.AddComponent<T>();
        return Instantiate(body);
    }

    public Body InstantiateBodyWith<T>(Vector2 position) where T : Component, new()
    {
        Body body = InstantiateBodyWith<T>();
        body.transform.Position = position;
        return body;
    }

    public Scene LoadScene(string filePath)
    {

        if (ProjectAssembly == null)
        {
            Log.Error("Cannot open scene when there is no project assembly loaded.");
            return null;
        }
        
        Log.Info($"Loading scene {filePath}...");
        
        ComponentConverter.patches.Clear();
        var options = new JsonSerializerOptions
        {
            IncludeFields = true,
            Converters = { new ComponentConverter(), new SceneConverter() },
            ReferenceHandler = ReferenceHandler.Preserve
        };
        if (LoadedScene != null)
            LoadedScene.PhysicsWorld.Clear();

        string json = AssetManager.ReadAllText(filePath);

        Scene scene = JsonSerializer.Deserialize<Scene>(json, options);
        scene.FileHandle = filePath;
        Time.Timers.Clear();

        foreach (Body body in scene.Bodys){

            foreach (Component comp in  body.Components){
                
                comp.body = body;

            }
        }

        foreach (var patch in ComponentConverter.patches)
        {
            var field = patch.comp.GetType().GetField(patch.field);
            foreach (var _body in scene.Bodys)
            {
                if (_body.ID == patch.targetBody)
                {
                    field.SetValue(patch.comp, _body.GetComponent(field.FieldType));
                }
            }
        }

        foreach (Body body in scene.Bodys){

            foreach (Component comp in  body.Components){

                foreach (var prop in comp.GetType().GetFields())
                {
                    if (prop.FieldType.IsAssignableTo(typeof(Body))){
                        if (prop.GetValue(comp) is QueuedBody queuedBody){
                            
                            foreach (var _body in scene.Bodys)
                            {
                                if (_body.ID == queuedBody.target){
                                    prop.SetValue(comp, _body);
                                }
                            }
                        }

                    }
                }

            }
        }



        LoadedScene = scene;

        foreach (Body body in scene.Bodys){

            foreach (Component comp in  body.Components){

                if (comp is Rigidbody rb){
                    rb.SetCollider(body.GetComponent<Collider>());
                }

            }
        }
        
        Log.Info($"Loaded scene {filePath}.");

        return scene;
    }
    public Body? AddSceneAsPrefab(string filePath, Vector2 position)
    {
        var options = new JsonSerializerOptions
        {
            IncludeFields = true,
            Converters = { new ComponentConverter(), new SceneConverter() }
        };

        string json = AssetManager.ReadAllText(filePath);

        Scene scene = JsonSerializer.Deserialize<Scene>(json, options);

        if (scene.Bodys.Count > 1){
            Console.WriteLine("WARNING: Prefabs cannot contain more then one object");
        }

        foreach (Body body in scene.Bodys){
            body.transform.Position = position;
            foreach (Component comp in  body.Components){
                comp.body = body;
                if (comp is Rigidbody rb){
                    rb.SetCollider(body.GetComponent<Collider>());
                }
            }
            LoadedScene.Bodys.Add(body);

            return body;
        }

        return null;

    }

    public Body Instantiate(Body body, Vector2 position)
    {
        return LoadedScene.Instantiate(body, position);
    }

    public Body Instantiate(Body body)
    {
        return LoadedScene.Instantiate(body);
    }

    public FrameData DrawFrame(bool update = true, bool fromeditor = false)
    {
        if (!fromeditor){
            Performance.CompletedTasks.Clear();
            Performance.ProcessingStack.Clear();
        }

        Performance.PushTask("Frame");
        List<string> keys = InvokeQueue.Keys.ToList();
        if (update){
            Performance.PushTask("Update");
            Performance.PushTask("Invoke Queue");
            for (int i = 0; i < keys.Count; i++)
            {

                if (Time.Completed(InvokeQueue[keys[i]].TimerId) && !InvokeQueue[keys[i]].Invoked){
                    InvokeQueue[keys[i]].Invoked = true;
                    InvokeQueue[keys[i]].method.DynamicInvoke(InvokeQueue[keys[i]].parameters);
                    InvokeQueue.Remove(keys[i]);
                }
            }
            Performance.PopTask();
            
            LoadedScene.Update();

            Performance.PopTask();
        }


        FrameData res = LoadedScene.Render();

        Performance.PopTask();

        if (!fromeditor){
            foreach (var task in Performance.ProcessingStack){
                Performance.CompletedTasks.Add(task);
            }
        }

        return res;

    }

    public virtual float GetFrameTime()
    {
        return 0;
    }

    public virtual float GetTime()
    {
        return 0;
    }
    
    
    public virtual bool PollInputDown(KeyCode key)
    {
        return false;
    }

    public virtual bool PollInputPressed(KeyCode key)
    {
        return false;
    }
    
    public virtual bool PollInputMouseDown(KeyCode key)
    {
        return false;
    }

    public virtual bool PollInputMousePressed(KeyCode key)
    {
        return false;
    }

    public static List<Type> GetSubclassesOf(Type baseType)
    {
        List<Assembly> assemblies = AppDomain.CurrentDomain.GetAssemblies().ToList();
        Dictionary<string, Assembly> finalAssemblies = new Dictionary<string, Assembly>();
        foreach (var assembly in assemblies)
        {
            if (!finalAssemblies.ContainsKey(assembly.FullName)){
                finalAssemblies.Add(assembly.FullName, assembly);
            }
        }
        return finalAssemblies
            .SelectMany(assembly => assembly.Value.GetTypes())
            .Where(type => type.IsSubclassOf(baseType) && !type.IsAbstract)
            .ToList();
    }

}

public class InvokeMethodData{
    public Action? method;
    public string TimerId = "";
    public object[] parameters = [];
    public bool Invoked;
}
