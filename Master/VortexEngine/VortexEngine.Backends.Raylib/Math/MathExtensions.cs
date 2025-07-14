namespace VortexEngine.Rendering.Backends.Math;

public static class MathExtensions
{
    public static Raylib_cs.Color ToRaylib(this Color color)
    {
        return new Raylib_cs.Color((byte)color.r, (byte)color.g, (byte)color.b, (byte)color.a);
    }
}