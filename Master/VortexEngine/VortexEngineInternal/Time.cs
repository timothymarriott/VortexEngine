namespace VortexEngine;

public static class Time
{

    public static float DeltaTime => VortexEngine.Master.GetFrameTime() * TimeScale;
    public static float TimeScale = 1;

    public static float ElapsedTime => VortexEngine.Master.GetTime() * TimeScale;

    public static Dictionary<string, Timer> Timers = new();


    public static void AddTimer(string Name, float Length){
        Timers.Add(Name, new Timer(){Length = Length, ElapsedTime = 0});
    }

    public static bool Completed(string Name){
        if (!Timers.ContainsKey(Name)) return false;

        if(Timers[Name].Length - Timers[Name].ElapsedTime <= 0){
            Timers.Remove(Name);
            return true;
        }
        return false;

    }

    public static void Update(){

        foreach (var value in Timers)
        {
            value.Value.ElapsedTime += DeltaTime;
        }

    }

    public class Timer{
        public float Length;
        public float ElapsedTime;
    }
    public static Stack<KeyValuePair<string, DateTime>> ProcessingStack = new();
    
    public static List<KeyValuePair<string, TimeSpan>> CompletedTasks = new();
    
    public static void PushTask(string taskName)
    {
        ProcessingStack.Push(new KeyValuePair<string, DateTime>(taskName, DateTime.Now));
    }

    public static void PopTask()
    {
        if (ProcessingStack.Count > 0)
        {
            var task = ProcessingStack.Pop();
            TimeSpan duration = DateTime.Now - task.Value;
            CompletedTasks.Add(new KeyValuePair<string, TimeSpan>(task.Key, duration));
        }
    }



}
