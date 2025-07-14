using System.Numerics;
using ImGuiNET;
using VortexEngine.Rendering;

namespace VortexEngine.Editor.Windows;

public class EditorSettingsWindow : EditorWindow
{
    public override string GetTitle()
    {
        return "Settings";
    }

    public override void DrawContent(VortexEngineEditor editor){
        
        #if DEBUG
        ImGui.Text("Debug v0.1");
        #else
        ImGui.Text("Release v0.1");
        #endif

        ImGui.Text("Project Path: " + VortexEngine.ProjectPath);
        ImGui.Text("Project Data Path: " + VortexEngine.ProjectDataPath);
        ImGui.BeginDisabled();

        System.Numerics.Vector2 vec = editor.GetWindow<SceneWindow>().SceneLocalMousePos;
        ImGui.InputFloat2("Local mouse pos", ref vec);
        ImGui.EndDisabled();

        ImGui.InputFloat("Time Scale", ref Time.TimeScale);

        ImGui.SliderFloat("Color Hue", ref editor.ThemeColor.X, -0.5f, 0.5f);
        ImGui.ColorEdit4("Theme Color", ref editor.ThemeColor);
        ImGui.Text("color: " + editor.ThemeColor.X.ToString());
        
        ImGui.Checkbox("Show Gizmos", ref editor.DrawGizmos);
        ImGui.Checkbox("Show Collider Gizmos", ref Debug.DrawColliders);

        ImGui.Text("Delta: " + Time.DeltaTime.ToString());

        if (Renderer.UITarget.id != -1){
            ImGui.Text("Ui target size " + Renderer.UITarget.Width + " " + Renderer.UITarget.Height);
        } else {
            ImGui.Text("No Ui target");
        }

        if (Renderer.target.id != -1)
            ImGui.Text("Render Size: " + Renderer.target.Width + " " + Renderer.target.Height);
        

        

    }
}
