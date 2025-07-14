namespace VortexEngine.Rendering;

public interface IImguiBackend
{
    void ImageSize(Texture texture, Vector2 size);
    void ImageSize(RenderTexture texture, Vector2 size);
    void ImageFit(Texture texture);
    void ImageFit(RenderTexture texture);
    void ReloadFonts();
}