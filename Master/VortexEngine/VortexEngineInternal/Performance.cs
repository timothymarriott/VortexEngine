using System.Diagnostics;
namespace VortexEngine;

public class TaskData
{
    public string Name { get; set; }
    public string Trace { get; set; }
    public long StartTimestamp { get; set; }
    public long EndTimestamp { get; set; }
    public List<TaskData> SubTasks { get; set; } = new List<TaskData>();

    public double DurationMS => (EndTimestamp - StartTimestamp) * 1000.0 / Stopwatch.Frequency;
}


public static class Performance
{
    
    public static Stack<TaskData> ProcessingStack = new Stack<TaskData>();

    public static List<TaskData> CompletedTasks = new List<TaskData>();
    public static List<TaskData> LastCompletedTasks = new List<TaskData>();
    
    public static void PushTask(string taskName)
    {
        #if DEBUG
        var newTask = new TaskData
        {
            Name = taskName,
        };
        newTask.StartTimestamp = Stopwatch.GetTimestamp();

        if (ProcessingStack.Count > 0)
        {
            var currentTask = ProcessingStack.Peek();
            currentTask.SubTasks.Add(newTask);
        }

        ProcessingStack.Push(newTask);
        #endif
    }

    public static void Populate()
    {
        LastCompletedTasks = CompletedTasks.ToList();
    }

    public static void PopTask()
    {
        #if DEBUG
        if (ProcessingStack.Count > 0)
        {
            var completedTask = ProcessingStack.Pop();
            completedTask.EndTimestamp = Stopwatch.GetTimestamp();

            if (ProcessingStack.Count == 0)
            {
                CompletedTasks.Add(completedTask);
            }
        }
        #endif
    }
    
}
