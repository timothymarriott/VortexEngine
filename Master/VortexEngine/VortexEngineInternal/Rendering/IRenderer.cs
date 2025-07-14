namespace VortexEngine.Rendering;

public interface IRenderer
{
    void DrawRectanglePro(Rect rec, Vector2 origin, float rotation, Color color);
    void DrawEllipse(Vector2 center, Vector2 radius, Color color);
    void DrawPolyLinesEx(Vector2 center, int sides, float radius, float rotation, float lineThick, Color color);
    void DrawLineEx(Vector2 startPos, Vector2 endPos, float thick, Color color);
    void DrawCircleV(Vector2 center, float radius, Color color);
    void DrawTexturePro(Texture texture, Rect source, Rect dest, Vector2 origin, float rotation, Color tint);
    
    void DrawRenderTexturePro(RenderTexture texture, Rect source, Rect dest, Vector2 origin, float rotation, Color tint);
    void UnloadTexture(Texture texture);
    void DrawRectangleLines(Vector2 pos, Vector2 size, Color color);
    RenderTexture LoadRenderTexture(Vector2 size);
    void EndTextureMode();
    void BeginTextureMode(RenderTexture target);
    void ClearBackground(Color color);
    void DrawTextEx(string text, Vector2 position, int fontSize, float spacing, Color tint);
    int GetTextureWidth(Texture texture);
    int GetTextureHeight(Texture texture);
    
    int GetRenderTextureWidth(RenderTexture texture);
    int GetRenderTextureHeight(RenderTexture texture);

    void UnloadRenderTexture(RenderTexture texture);

    Texture LoadTexture(string path);

    void TextureFlipVertical(Texture texture);
    
}