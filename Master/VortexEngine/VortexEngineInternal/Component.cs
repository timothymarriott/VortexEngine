using System.Text.Json.Serialization;
using VortexEngine.Rendering;

namespace VortexEngine;

[System.Serializable]
public abstract class Component
{

    [Serializable]
    public struct ComponentData
    {
        public string ScriptName;
        public List<VariableData> Variables;
    }

    [Serializable]
    public struct VariableData
    {
        public string Name;
        public string Type;
        public string Value;
    }

    [HideInInspector, JsonIgnore]
    public Body body;

    [JsonIgnore]
    public Transform transform { get {
        if (body != null) return body.transform;
        throw new Exception("No body on component");
    }}

    [HideInInspector, JsonIgnore]
    public string ScriptName = "";

    [HideInInspector, JsonIgnore]
    public bool Started = false;

    public virtual void Start()
    {

    }

    public virtual void Update()
    {

    }

    public virtual void Draw()
    {

    }

    public virtual void DrawUI()
    {
        
    }

    public virtual void DrawGizmos()
    {

    }

    public virtual void OnDestroyed(){

    }

    public virtual void Awake(){

    }

    public virtual void OnValidate(){

    }

    public virtual void DrawDebugGui(){

    }

    public virtual void DrawGui(){

        EditorGui.DrawObject(this);
        return;

    }

    public ComponentData GetData()
    {
        return new ComponentData()
        {
            ScriptName = ScriptName,
            Variables = new List<VariableData>()
        };
    }

    public T? GetComponent<T>() where T : Component
    {
        if (body == null){
            return null;
        }
        foreach (Component comp in body.Components)
        {

            if (comp is T)
            {
                return comp as T;
            }

        }

        return null;

    }

    public T AddComponent<T>() where T : Component, new(){
        return body.AddComponent<T>();
    }

    public T AddComponent<T>(Action<T> initCode) where T : Component, new(){
        T result = body.AddComponent<T>();
        initCode(result);
        return result;
    }

    public void Invoke(Action method, float delay, params object[] parameters){
        string timerID = "Internal_Invoke_" + method.Method.Name + "_" + Guid.NewGuid().ToString();
        Time.AddTimer(timerID, delay);

        string paramsString = "";


        int index = 0;
        foreach (var param in method.Method.GetParameters())
        {
            paramsString += $"{param.ParameterType.Name} {param.Name} = {parameters[index]}";
            if (index < parameters.Length - 1)
            {
                paramsString += ", ";
            }
            index++;
        }
        
        Log.Info($"Invoking method \"{method.Method.Name}({paramsString})\" in {delay} seconds");
        VortexEngine.Master.InvokeQueue.Add(timerID, new InvokeMethodData(){method = method, TimerId = timerID, parameters = parameters});

    }

    public Body Instantiate(Body body, Vector2 position)
    {
        
        return VortexEngine.Master.LoadedScene.Instantiate(body, position);
    }

    public Body Instantiate(Body body)
    {
        return VortexEngine.Master.LoadedScene.Instantiate(body);
    }

    public void Destroy(Body body){
        body.Destroy(body);
    }

}
