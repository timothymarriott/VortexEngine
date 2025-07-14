using ImGuiNET;

namespace VortexEngine.Editor.Windows;

public class TimerWindow : EditorWindow
{
    public override string GetTitle()
    {
        return "Timers";
    }

    public override void DrawContent(VortexEngineEditor editor)
    {
        foreach(KeyValuePair<string, Time.Timer> timer in Time.Timers){
            ImGui.Text(timer.Key + " - " + (timer.Value.Length - timer.Value.ElapsedTime).ToString());
        }
    }
}