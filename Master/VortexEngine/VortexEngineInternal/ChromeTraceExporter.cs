using System.Diagnostics;

namespace VortexEngine;

using System.Text.Json;

public static class ChromeTraceExporter
{
    
    static double timestampToMicro(long ticks) => ticks * 1_000_000.0 / Stopwatch.Frequency;
    
    static void AddEvent(List<object> events, TaskData task)
    {
        double startMicro = timestampToMicro(task.StartTimestamp);
        double durMicro = timestampToMicro(task.EndTimestamp - task.StartTimestamp);

        events.Add(new
        {
            name = task.Name,
            ph = "X", // Complete event
            ts = startMicro,
            dur = durMicro,
            pid = 1,
            tid = 1,
            args = new
            {
                trace = task.Trace
            }
        });

        foreach (var sub in task.SubTasks)
            AddEvent(events, sub);
    }
    
    public static string ExportToChromeTrace(List<TaskData> tasks)
    {
        var events = new List<object>();
        
        foreach (var task in tasks)
            AddEvent(events, task);

        var options = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        string json = JsonSerializer.Serialize(new { traceEvents = events }, options);
        return json;
    }
}
