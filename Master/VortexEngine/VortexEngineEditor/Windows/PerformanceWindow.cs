using ImGuiNET;

namespace VortexEngine.Editor.Windows;

public class PerformanceWindow : EditorWindow
{

    public bool Paused = false;
    
    public override string GetTitle()
    {
        return "Performance";
    }

    public override void DrawContent(VortexEngineEditor editor)
    {


        if (ImGui.Button("Pause"))
        {

            string trace = ChromeTraceExporter.ExportToChromeTrace(Performance.LastCompletedTasks);

            var res = NativeFileDialogSharp.Dialog.FileSave();

            if (res != null && res.IsOk)
            {
                File.WriteAllText(res.Path, trace);
            }
            
            Paused = !Paused;
        }

        foreach (var task in Performance.LastCompletedTasks)
        {
            DrawTaskTreeWithMerging(task);
        }
        
        
        if (Paused)
            if (Performance.LastCompletedTasks.Count > 0)
                DrawTask(Performance.LastCompletedTasks[0], 0);
        
    }

    const float barHeight = 45;
    private const float pad = 5;
    public static float DrawTask(TaskData task, float offset, float yoffset = 0, int siblings = 1)
    {
        float FullWidth = ImGui.GetContentRegionAvail().X;

        float frac = (float)(task.DurationMS / Performance.LastCompletedTasks[0].DurationMS);
        float targetWidth = FullWidth * frac;
        

        
        ImDrawListPtr draw = ImGui.GetWindowDrawList();
        Vector2 rectMin = new Vector2(offset + pad, yoffset + pad) + ImGui.GetCursorScreenPos();
        Vector2 rectMax = new Vector2(offset + targetWidth - pad, barHeight + yoffset - pad) + ImGui.GetCursorScreenPos();

        if (rectMax.x < rectMin.x)
        {
            rectMax.x += pad * 2;
        }
        draw.AddRectFilled( rectMin, rectMax, ImGui.GetColorU32(Color.Red), 2);
        draw.AddRect(rectMin, rectMax, ImGui.GetColorU32(Color.Gray), 2);
        draw.AddText(rectMin, ImGui.GetColorU32(Color.White), task.Name);
        
        
        if (new Rect(rectMin, rectMax).Contains( ImGui.GetMousePos()))
        {
            if (ImGui.BeginTooltip())
            {
                ImGui.Text(task.Name);
                ImGui.EndTooltip();
            }
        }
        
        float currOffset = offset;
        foreach (var subTask in task.SubTasks)
        {
            currOffset += DrawTask(subTask, currOffset, yoffset + barHeight, task.SubTasks.Count);
        }

        return targetWidth;

    }

    private static void DrawTaskTreeWithMerging(TaskData task)
    {
        if (task.SubTasks.Count > 0)
        {
            if (ImGui.TreeNodeEx($"{task.Name} | {task.Trace} {task.DurationMS:F2}ms###{task.Name}@{task.Trace}"))
            {
                ImGui.Indent();
                foreach (var subTask in task.SubTasks)
                {
                    DrawTaskTreeWithMerging(subTask);
                }
                ImGui.Unindent();
                ImGui.TreePop();
            }

        }
        else
        {
            ImGui.Text($"{task.Name} | {task.Trace} {task.DurationMS:F2}ms");
        }
        
        

    }

}