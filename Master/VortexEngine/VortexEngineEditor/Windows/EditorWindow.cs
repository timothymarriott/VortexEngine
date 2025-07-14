using ImGuiNET;

namespace VortexEngine.Editor.Windows;

public abstract class EditorWindow
{
    
    public virtual void DrawContent(VortexEngineEditor editor)
    {
        
    }

    public virtual void Draw()
    {
        if (ImGui.Begin(GetTitle(), GetFlags()))
        {
            DrawContent(VortexEngineEditor.Editor);
        }
        ImGui.End();
    }

    public virtual string GetTitle()
    {
        return this.GetType().Name;
    }

    public virtual ImGuiWindowFlags GetFlags()
    {
        return ImGuiWindowFlags.None;
    }
    
}