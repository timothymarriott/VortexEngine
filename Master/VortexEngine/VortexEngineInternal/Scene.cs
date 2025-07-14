using VortexEngine.Rendering;
using Genbox.VelcroPhysics.Dynamics;
using Genbox.VelcroPhysics.Utilities;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace VortexEngine;

public class Scene
{

    public string Name;

    public string FileHandle;

    public List<Body> Bodys = new List<Body>();

    public World PhysicsWorld = new World(new Vector2(0, -9.82f));

    public List<Body> InstantiateCache = new List<Body>();

    List<Body> DestroyCache = new List<Body>();

    public void Start()
    {
        
        ConvertUnits.SetDisplayUnitToSimUnitRatio(48f);

        foreach (Body body in Bodys)
        {
            if (!body.Started){
                body.Start();
                body.Started = true;
            }
        }
    }

    public void Update()
    {

        Performance.PushTask("Scene Update");
        Performance.PushTask("Time Update");
        Time.Update();
        Performance.PopTask();


        Performance.PushTask("Physics Step");
        PhysicsWorld.Step(Time.DeltaTime);

        Performance.PopTask();



        Performance.PushTask("Instantiation");
        foreach (Body body in InstantiateCache)
        {


            Bodys.Add(body);
            
            Log.Debug($"Instantiated {body.Name}.", DebugLevel.Medium);

        }
        InstantiateCache.Clear();
        Performance.PopTask();

        Performance.PushTask("Body Updates");
        foreach (Body body in Bodys)
        {
            
            Performance.PushTask($"Update {body.Name}");
            
            if (!body.Started){
                Performance.PushTask("Initialization");
                body.Awake();
                body.Start();
                
                body.Started = true;
                Performance.PopTask();
            }
            Performance.PushTask("Update");
            body.Update();
            Performance.PopTask();
            Performance.PopTask();
        }
        Performance.PopTask();


        foreach (Body body in DestroyCache){

            body.OnDestroyed();
            Bodys.Remove(body);
        }
        DestroyCache.Clear();


        Performance.PopTask();


    }

    public void DrawGizmos()
    {
        Performance.PushTask("Scene Gizmos");

        foreach (Body body in Bodys)
        {

            body.DrawGizmos();
        }

        Performance.PopTask();


    }

    public Body Instantiate(Body body, Vector2 position)
    {
        body.transform.Position = position;
        return Instantiate(body);
    }

    public Body Instantiate(Body body)
    {
        Log.Debug($"Instantiating {body.Name}...", DebugLevel.Medium);
        InstantiateCache.Add(body);

        return body;
    }

    public FrameData Render()
    {

        Performance.PushTask("Scene Rendering");

        if (FindObjectOfType<Camera>() == null) return new FrameData();


        FrameData res = FindObjectOfType<Camera>().Render();

        Performance.PopTask();

        return res;

    }

    public List<T> FindObjectsOfType<T>() where T : Component
    {
        Performance.PushTask("FindObjectsOfType");
        List<T> result = new List<T>();

        foreach (Body body in Bodys)
        {
            if (body.GetComponent<T>() != null)
            {
                result.Add(body.GetComponent<T>());
            }
        }

        foreach (Body body in InstantiateCache)
        {
            if (body.GetComponent<T>() != null)
            {
                result.Add(body.GetComponent<T>());
            }
        }

        return result;

    }

    public T? FindObjectOfType<T>() where T : Component
    {

        try {
            foreach (Body body in Bodys)
            {
                if (body.GetComponent<T>() != null)
                {
                    return body.GetComponent<T>();
                }
            }

            foreach (Body body in InstantiateCache)
            {
                if (body.GetComponent<T>() != null)
                {
                    return body.GetComponent<T>();
                }
            }

        } catch{

        }

        return null;

    }

    public void Destroy(Body body){
        DestroyCache.Add(body);
    }

    public void Save()
    {
        var options = new JsonSerializerOptions
        {
            IncludeFields = true,               // Include fields in serialization
            WriteIndented = true,                // For better readability
            Converters = { new ComponentConverter(), new SceneConverter() }, // Use the custom converter for Components
            ReferenceHandler = ReferenceHandler.Preserve,
            
        };


        string json = JsonSerializer.Serialize(this, options);

        File.WriteAllText(VortexEngine.ProjectDataPath + FileHandle, json);

        Console.WriteLine(FileHandle);

    }




}
