using System.Numerics;
using ImGuiNET;
using VortexEngine.Rendering;

namespace VortexEngine.Editor.Windows;

public class SceneSettingsWindow : EditorWindow
{
    public override string GetTitle()
    {
        return "Scene Settings";
    }

    public override void DrawContent(VortexEngineEditor editor){
        
        System.Numerics.Vector2 val = editor.GetWindow<SceneWindow>().Size;
        ImGui.DragFloat2("Size", ref val);
        editor.GetWindow<SceneWindow>().Size = new Vector2I((int)val.X, (int)val.Y);

        System.Numerics.Vector3 col = new Vector3(editor.GetWindow<SceneWindow>().BackgroundColor.r / 255f, editor.GetWindow<SceneWindow>().BackgroundColor.g / 255f, editor.GetWindow<SceneWindow>().BackgroundColor.b / 255f);
        ImGui.ColorEdit3("Clear Color", ref col);
        editor.GetWindow<SceneWindow>().BackgroundColor = new Color(col.X * 255f, col.Y * 255f, col.Z * 255f);

    }
}
