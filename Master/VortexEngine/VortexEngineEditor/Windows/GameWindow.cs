using ImGuiNET;
using rlImGui_cs;
using VortexEngine.Rendering;
using VortexEngine.Rendering.Backends;

namespace VortexEngine.Editor.Windows;

public class GameWindow : EditorWindow
{
    
    public RenderTexture FrameTexture = RenderTexture.Null;
    
    public override string GetTitle()
    {
        return "Game";
    }

    public override void DrawContent(VortexEngineEditor editor)
    {
        Render();
    }
    
    public void Render(){
        if (FrameTexture.id != -1){
            Vector2 windowSize = ImGui.GetContentRegionAvail();

            float aspectRatio = (float)FrameTexture.Height / (float)FrameTexture.Width;
            float invertedAspectRatio =  (float)FrameTexture.Width / (float)FrameTexture.Height;

            Vector2 displaySize = new Vector2(windowSize.x, windowSize.x * aspectRatio);

            if (windowSize.x * aspectRatio > windowSize.y){
                displaySize = new Vector2(windowSize.y * invertedAspectRatio, windowSize.y);
            }
            
            if (Renderer.backend is RaylibRenderer renderer)
            {
                rlImGui.ImageSize(renderer.GetRenderTexture(FrameTexture).Texture, displaySize);
            }
            
        }

    }

    public override ImGuiWindowFlags GetFlags()
    {
        return ImGuiWindowFlags.NoScrollbar;
    }
}