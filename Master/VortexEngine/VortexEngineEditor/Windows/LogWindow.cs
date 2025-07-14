using ImGuiNET;

namespace VortexEngine.Editor.Windows;

public class LogWindow : EditorWindow
{
    public override string GetTitle()
    {
        return "Log";
    }

    public override void DrawContent(VortexEngineEditor editor)
    {
        ImGui.Text(ReverseLines((StringWriter)Log.logOutput));
    }
    
    public string ReverseLines(StringWriter writer)
    {
        string[] lines = writer.ToString().Split(Environment.NewLine);

        string[] reversedLines = lines.Reverse().ToArray();

        string result = string.Join(Environment.NewLine, reversedLines);

        return result;
    }
}