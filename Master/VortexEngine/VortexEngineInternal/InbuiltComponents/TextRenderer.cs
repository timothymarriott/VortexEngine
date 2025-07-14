using VortexEngine.Rendering;

namespace VortexEngine;

public class TextRenderer : Component
{
    public string Text = "";
    public int FontSize;
    public Color color;

    public override void Update()
    {
        
    }

    public override void DrawUI()
    {
        Renderer.DrawText(Text, transform.Position, FontSize, color, transform.Rotation);
    }
}
