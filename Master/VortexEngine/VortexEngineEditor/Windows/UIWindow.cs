using VortexEngine.Rendering;

namespace VortexEngine.Editor.Windows;

public class UIWindow : EditorWindow
{
    public override string GetTitle()
    {
        return "UI";
    }

    public override void DrawContent(VortexEngineEditor editor)
    {
        if (Renderer.UITarget.id != -1){
            Renderer.guiBackend.ImageFit(Renderer.UITarget);
        }
    }
}