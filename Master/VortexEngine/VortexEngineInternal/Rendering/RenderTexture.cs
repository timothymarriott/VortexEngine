namespace VortexEngine.Rendering;

public class RenderTexture
{
    public static RenderTexture Null => new RenderTexture() { id = -1 };
    public int id = -1;
    public int Width => Renderer.backend.GetRenderTextureWidth(this);

    public int Height => Renderer.backend.GetRenderTextureHeight(this);

    public Vector2 Size => new(Renderer.backend.GetRenderTextureWidth(this), Renderer.backend.GetRenderTextureHeight(this));

}