using System.Text.Json.Serialization;
using VortexEngine.Internal;

namespace VortexEngine;

public class Body
{

    [Serializable]
    public struct BodyData
    {
        public string ID;
        public string Name;
        public List<Component.ComponentData> Components;
        public string Parent;

    }

    public string ID = Guid.Empty.ToString();
    public string Name = "New Body";

    public List<Component> Components = new();

    public string? Parent;
    [JsonIgnore]
    public List<string> Children = new();

    [JsonIgnore]
    public bool Started = false;

    public int SortingOrder = 0;

    public Body(Action<Body> initCode){
        AddComponent<Transform>();
        initCode(this);
    }

    public Body(){
        AddComponent<Transform>();
        ID = Guid.NewGuid().ToString();
    }
    
    public void AddChild(Body child)
    {
        Children.Add(child.ID);
        child.Parent = ID;
    }

    public void SetParent(Body? parent)
    {
        if (parent != null)
            Parent = parent.ID;
        else
            Parent = "";
    }

    internal void Start()
    {
        ID = Guid.NewGuid().ToString();
        for (int i = 0; i < Components.Count; i++)
        {
            Component comp = Components[i];

            comp.Started = true;
            comp.Start();
        }
    }

    internal void Update()
    {

        for (int i = 0; i < Components.Count; i++)
        {
            Component comp = Components[i];

            if (!comp.Started){
                comp.Started = true;
                comp.Awake();
                comp.Start();
            }
            try {
                comp.Update();
            } catch (Exception e){
                Console.WriteLine("ERROR: error of type " + e.GetType().Name + " in update function for " + Name + " in " + comp.GetType().Name + " error is " + e.Message);
                if (comp.GetType().Assembly != VortexEngine.ProjectAssembly){
                    throw;
                }
            }

        }

    }

    public void Draw()
    {
        foreach (Component comp in Components)
        {
            comp.Draw();
        }
    }
    
    public void DrawUI()
    {
        foreach (Component comp in Components)
        {
            comp.DrawUI();
        }
    }

    public void DrawGui()
    {
        foreach (Component comp in Components)
        {
            comp.DrawGui();
        }
    }

    public void DrawDebugGui()
    {
        foreach (Component comp in Components)
        {
            comp.DrawDebugGui();
        }
    }

    public void Awake()
    {
        foreach (Component comp in Components)
        {
            comp.Awake();
        }
    }

    public void DrawGizmos()
    {
        foreach (Component comp in Components)
        {
            comp.DrawGizmos();
        }
    }

    public void OnDestroyed()
    {
        foreach (Component comp in Components)
        {
            comp.OnDestroyed();
        }
    }

    #region Data

    public BodyData GetData()
    {
        return new BodyData()
        {
            ID = ID,
            Name = Name,
            Components = Components.GetData()
        };
    }

    #endregion

    #region Transform

    [JsonIgnore]
    public Transform transform => GetTransform();

    protected Transform GetTransform()
    {
        Transform? trans = GetComponent<Transform>();
        if (trans == null) {
            throw new Exception("No Transform on object");
        }
        return trans;
    }

    #endregion

    #region ComponentManager

    public T? GetComponent<T>() where T : Component
    {

        foreach (Component comp in Components)
        {

            if (comp is T)
            {
                return comp as T;
            }

        }

        return null;

    }

    public Component? GetComponent(Type t)
    {

        foreach (Component comp in Components)
        {

            if (comp.GetType() == t)
            {
                return comp;
            }

        }

        return null;

    }

    public T AddComponent<T>() where T : Component, new()
    {
        T result = new T();
        Components.Add(result);
        result.body = this;

        return result;
    }

    public Component AddComponent(Type type)
    {
        Component result = (Component)Activator.CreateInstance(type);
        Components.Add(result);
        result.body = this;

        return result;
    }

    public void Destroy(Body body){
        VortexEngine.Master.LoadedScene.Destroy(body);
    }

    #endregion

}
