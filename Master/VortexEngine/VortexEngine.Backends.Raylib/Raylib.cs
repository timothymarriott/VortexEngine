using VortexEngine;
using VortexEngine.Rendering.Backends.Math;

namespace VortexEngine.Rendering.Backends;

using Raylib_cs;

using Color = global::VortexEngine.Color;

public class RaylibRenderer : IRenderer
{

    private Dictionary<int, Texture2D> textures = new Dictionary<int, Texture2D>();
    private Dictionary<int, RenderTexture2D> renderTextures = new Dictionary<int, RenderTexture2D>();
    
    public void DrawRectanglePro(Rect rec, Vector2 origin, float rotation, Color color)
    {
        Raylib.DrawRectanglePro(new Rectangle(rec.Position, rec.Size), origin, rotation, color.ToRaylib());
    }

    public void DrawEllipse(Vector2 center, Vector2 radius, Color color)
    {
        Raylib.DrawEllipse((int)center.x, (int)center.y, radius.x, radius.y, color.ToRaylib());
    }

    public void DrawPolyLinesEx(Vector2 center, int sides, float radius, float rotation, float lineThick, Color color)
    {
        Raylib.DrawPolyLinesEx(center, sides, radius, rotation, lineThick, color.ToRaylib());
    }

    public void DrawLineEx(Vector2 startPos, Vector2 endPos, float thick, Color color)
    {
        Raylib.DrawLineEx(startPos, endPos, thick, color.ToRaylib());
    }

    public void DrawCircleV(Vector2 center, float radius, Color color)
    {
        Raylib.DrawCircleV(center, radius, color.ToRaylib());
    }

    public void DrawTexturePro(Texture texture, Rect source, Rect dest, Vector2 origin, float rotation, Color tint)
    {
        Raylib.DrawTexturePro(GetTexture(texture), new Rectangle(source.Position, source.Size), new Rectangle(dest.Position, dest.Size), origin, rotation, tint.ToRaylib());
    }

    public void DrawRenderTexturePro(RenderTexture texture, Rect source, Rect dest, Vector2 origin, float rotation, Color tint)
    {
        Raylib.DrawTexturePro(GetRenderTexture(texture).Texture, new Rectangle(source.Position, source.Size), new Rectangle(dest.Position, dest.Size), origin, rotation, tint.ToRaylib());

    }

    public void UnloadTexture(Texture texture)
    {
        if (texture.id == -1) return;
        Raylib.UnloadTexture(GetTexture(texture));
        textures.Remove(texture.id);
    }

    public void DrawRectangleLines(Vector2 pos, Vector2 size, Color color)
    {
        Raylib.DrawRectangleLines((int)pos.x, (int)pos.y, (int)size.x, (int)size.y, color.ToRaylib());
    }

    public RenderTexture LoadRenderTexture(Vector2 size)
    {
        RenderTexture2D tex = Raylib.LoadRenderTexture((int)size.x, (int)size.y);
        Log.Info("Loaded render texture: " + tex.Id);
        renderTextures.Add((int)tex.Id, tex);
        return new RenderTexture() { id = (int)tex.Id };
    }

    public void EndTextureMode()
    {
        Raylib.EndTextureMode();
    }

    public void BeginTextureMode(RenderTexture target)
    {
        Raylib.BeginTextureMode(GetRenderTexture(target));
    }

    public void ClearBackground(Color color)
    {
        Raylib.ClearBackground(color.ToRaylib());
    }

    public void DrawTextEx(string text, Vector2 position, int fontSize, float spacing, Color tint)
    {
        Raylib.DrawTextEx(Raylib.GetFontDefault(), text, position, fontSize, spacing, tint.ToRaylib());
    }

    public int GetTextureWidth(Texture texture)
    {
        return GetTexture(texture).Width;
    }

    public int GetTextureHeight(Texture texture)
    {
        return GetTexture(texture).Height;
    }

    public int GetRenderTextureWidth(RenderTexture texture)
    {
        if (texture.id == -1) return 0;
        return GetRenderTexture(texture).Texture.Width;
    }

    public int GetRenderTextureHeight(RenderTexture texture)
    {
        if (texture.id == -1) return 0;
        return GetRenderTexture(texture).Texture.Height;
    }

    public void UnloadRenderTexture(RenderTexture texture)
    {
        if (texture.id == -1) return;
        Raylib.UnloadRenderTexture(GetRenderTexture(texture));
        renderTextures.Remove(texture.id);
    }

    public Texture LoadTexture(string path)
    {
        Texture2D _tex = Raylib.LoadTexture(path);
        
        textures.Add((int)_tex.Id, _tex);

        return new Texture() { id = (int)_tex.Id };
    }

    public void TextureFlipVertical(Texture texture)
    {
        Image img = Raylib.LoadImageFromTexture(GetTexture(texture));
        Raylib.ImageFlipVertical(ref img);
        Raylib.UnloadTexture(GetTexture(texture));
        textures[texture.id] = Raylib.LoadTextureFromImage(img);
        Raylib.UnloadImage(img);
    }


    public Texture2D GetTexture(Texture texture)
    {
        if (texture.id == -1)
        {
            Log.Error("Trying to load null texture");
            throw new Exception();
        }
        return textures[texture.id];
    }
    
    public RenderTexture2D GetRenderTexture(RenderTexture texture)
    {
        if (texture.id == -1)
        {
            Log.Error("Trying to load null texture");
            throw new Exception();
        }
        return renderTextures[texture.id];
    }
}