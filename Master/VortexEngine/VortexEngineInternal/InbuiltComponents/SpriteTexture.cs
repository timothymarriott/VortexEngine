using ImGuiNET;
using VortexEngine.Internal.AssetManagement;
using VortexEngine.Rendering;

namespace VortexEngine;

public class SpriteTexture : Component
{

    [HideInInspector]
    public string TextureID = "";

    public Color tint = Color.White;

    public int TextureWidth => AssetManager.GetTexture(TextureID).Width;
    public int TextureHeight => AssetManager.GetTexture(TextureID).Height;

    public Vector2 TextureSize => new(TextureWidth, TextureHeight);


    public Rect Region;

    public override void Draw()
    {
        if (Region.Size.Magnitude() == 0)
            Renderer.DrawTexture(AssetManager.GetTexture(TextureID), transform.Position, TextureSize * transform.Scale, transform.Rotation, tint);
        else
            Renderer.DrawTexture(AssetManager.GetTexture(TextureID), transform.Position, Region.Size * transform.Scale, Region, transform.Rotation, tint);
    }

    public override void DrawGui()
    {
        base.DrawGui();
        
        Texture texture = AssetManager.GetTexture(TextureID);
        Renderer.guiBackend.ImageSize(texture, new Vector2(64, 64));

        int selected = 0;
        if (AssetManager.AvailableImageIds.Contains(TextureID)){
            selected = AssetManager.AvailableImageIds.IndexOf(TextureID) + 1;
        }
        string[] items = { "None" };
        items = items.Concat(AssetManager.AvailableImageIds.ToArray()).ToArray();
        ImGui.Combo("Texture", ref selected, items, AssetManager.AvailableImageIds.Count);

        TextureID = items[selected];

    }

}
