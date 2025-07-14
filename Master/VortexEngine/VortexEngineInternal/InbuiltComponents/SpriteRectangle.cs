using VortexEngine.Rendering;

namespace VortexEngine;

public class SpriteRectangle : Component
{

    public Color tint;

    public override void Draw()
    {

        Renderer.DrawRectangle(transform.Position, transform.Scale, transform.Rotation, tint);

    }

}
