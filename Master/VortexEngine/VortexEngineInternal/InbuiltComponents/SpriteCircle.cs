using VortexEngine.Rendering;

namespace VortexEngine;

public class SpriteCircle : Component
{

    public Color tint;

    public override void Draw()
    {

        Renderer.DrawCircle(transform.Position, transform.Scale, tint);

    }

}
