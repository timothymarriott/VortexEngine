namespace VortexEngine;

[Serializable]
public partial struct Color
{

    public float r;
    public float g;
    public float b;
    public float a;

    public Color(float r, float g, float b)
    {
        this.r = r;
        this.g = g;
        this.b = b;
        this.a = 255;
    }

    public Color(float r, float g, float b, float a)
    {
        this.r = r;
        this.g = g;
        this.b = b;
        this.a = a;
    }

    public static Color Black = new(0, 0, 0);
    public static Color Blue = new(0, 0, 255);
    public static Color Clear = new(0, 0, 0, 0);
    public static Color Cyan = new(0, 255, 255);
    public static Color Gray = new(255 / 2f, 255 / 2f, 255 / 2f);
    public static Color Green = new(0, 255, 0);
    public static Color Magenta = new(255, 0, 255);
    public static Color Red = new(255, 0, 0);
    public static Color White = new(255, 255, 255);
    public static Color Yellow = new(255, 255 * 0.92f, 255 * 0.016f);
    
    public static implicit operator System.Numerics.Vector4(Color color)
    {
        return new System.Numerics.Vector4(color.r / 255, color.g / 255, color.b / 255, color.a / 255);
    }

    public static implicit operator Color(System.Numerics.Vector4 color)
    {
        return new Color(color.X * 255, color.Y * 255, color.Z * 255, color.W * 255);
    }

    public static Color operator *(Color left, Color right)
    {
        float r = 255 * ((left.r / 255) * (right.r / 255));
        float g = 255 * ((left.g / 255) * (right.g / 255));
        float b = 255 * ((left.b / 255) * (right.b / 255));
        float a = 255 * ((left.a / 255) * (right.a / 255));
        return new Color(r, g, b, a);
    }
    
    public override string ToString()
    {
        return $"({r}, {g}, {b}, {a})";
    }

    public static Color FromGuid(string guid){

        Guid _guid = Guid.Parse(guid);

        byte[] bytes = _guid.ToByteArray();

        int red = bytes[0];
        int green = bytes[1];
        int blue = bytes[2];

        return new Color(red, green, blue, 255);
    }

}
