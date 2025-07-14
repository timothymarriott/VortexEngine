namespace VortexEngine.Rendering;

public class Texture
{

    public static Texture Null => new Texture() { id = -1 };
    
    public int id = -1;

    public int Width => Renderer.backend.GetTextureWidth(this);

    public int Height => Renderer.backend.GetTextureHeight(this);

    public Vector2 Size => new(Renderer.backend.GetTextureWidth(this), Renderer.backend.GetTextureHeight(this));
}