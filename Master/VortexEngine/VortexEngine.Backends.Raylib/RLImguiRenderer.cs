using rlImGui_cs;

namespace VortexEngine.Rendering.Backends;

public class RLImguiRenderer : IImguiBackend
{
    public void ImageSize(Texture texture, Vector2 size)
    {
        if (Renderer.backend is RaylibRenderer raylibRenderer)
        {
            rlImGui.ImageSize(raylibRenderer.GetTexture(texture), size);
        }
    }

    public void ImageSize(RenderTexture texture, Vector2 size)
    {
        if (Renderer.backend is RaylibRenderer raylibRenderer)
        {
            rlImGui.ImageRenderTexture(raylibRenderer.GetRenderTexture(texture));
        }
    }

    public void ImageFit(Texture texture)
    {
        throw new NotImplementedException();
    }

    public void ImageFit(RenderTexture texture)
    {
        if (Renderer.backend is RaylibRenderer raylibRenderer)
        {
            rlImGui.ImageRenderTextureFit(raylibRenderer.GetRenderTexture(texture), true);
        }
    }

    public void ReloadFonts()
    {
        rlImGui.ReloadFonts();
    }
}