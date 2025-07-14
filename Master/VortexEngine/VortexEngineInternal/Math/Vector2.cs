using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace VortexEngine;

[Serializable]
public struct Vector2
{

    public static readonly Vector2 Zero = new(0, 0);
    public static readonly Vector2 One = new(1, 1);
    public static readonly Vector2 Up = new(0, 1);
    public static readonly Vector2 Down = new(0, -1);
    public static readonly Vector2 Right = new(1, 0);
    public static readonly Vector2 Left = new(-1, 0);

    public float x { get; set; }
    public float y { get; set; }

    public Vector2(float x, float y)
    {
        this.x = x;
        this.y = y;
    }

    public float Magnitude()
    {
        return (float)Math.Sqrt(x * x + y * y);
    }

    public void Normalize()
    {
        float magnitude = Magnitude();
        if (magnitude > 0)
        {
            x /= magnitude;
            y /= magnitude;
        }
    }

    public Vector2 Normalized()
    {
        float magnitude = Magnitude();

        if (magnitude > 0)
        {
            return new Vector2(x / magnitude, y / magnitude);
        }
        return new Vector2(0, 0);
    }
    
    [JsonIgnore]
    public Vector2 normalized => Normalized();

    public float Distance(Vector2 other)
    {
        return (this - other).Magnitude();
    }

    public static Vector2 operator +(Vector2 a, Vector2 b)
    {
        return new Vector2(a.x + b.x, a.y + b.y);
    }
    
    public static bool operator ==(Vector2 a, System.Numerics.Vector2 b)
    {
        return a.x == b.X && a.y == b.Y;
    }
    
    public static bool operator !=(Vector2 a, System.Numerics.Vector2 b)
    {
        return !(a.x == b.X && a.y == b.Y);
    }
    
    public static Vector2 operator +(System.Numerics.Vector2 a, Vector2 b)
    {
        return new Vector2(a.X + b.x, a.X + b.y);
    }
    
    public static Vector2 operator +(Vector2 a, System.Numerics.Vector2 b)
    {
        return new Vector2(a.x + b.X, a.y + b.Y);
    }
    
    public static Vector2 operator -(Vector2 a, Vector2 b)
    {
        return new Vector2(a.x - b.x, a.y - b.y);
    }

    public static Vector2 operator *(Vector2 a, float scalar)
    {
        return new Vector2(a.x * scalar, a.y * scalar);
    }

    public static Vector2 operator *(Vector2 a, Vector2 scalar)
    {
        return new Vector2(a.x * scalar.x, a.y * scalar.y);
    }

    public static Vector2 operator /(Vector2 a, float scalar)
    {
        if (scalar != 0)
        {
            return new Vector2(a.x / scalar, a.y / scalar);
        }
        throw new DivideByZeroException("Division by zero is not allowed.");
    }

    public static Vector2 operator /(Vector2 a, Vector2 scalar)
    {

        return new Vector2(a.x / scalar.x, a.y / scalar.y);
    }

    public static implicit operator System.Numerics.Vector2(Vector2 customVector)
    {
        return new System.Numerics.Vector2(customVector.x, customVector.y);
    }

    public static implicit operator Microsoft.Xna.Framework.Vector2(Vector2 customVector)
    {
        return new Microsoft.Xna.Framework.Vector2(customVector.x, customVector.y);
    }

    public static implicit operator Vector2(Microsoft.Xna.Framework.Vector2 customVector)
    {
        return new Vector2(customVector.X, customVector.Y);
    }

    public static implicit operator Vector2(System.Numerics.Vector2 customVector)
    {
        return new Vector2(customVector.X, customVector.Y);
    }

    public override string ToString()
    {
        return $"({x}, {y})";
    }

    public static Vector2 RotateAboutOrigin(Vector2 point, Vector2 origin, float rotation)
    {
        return (Vector2)System.Numerics.Vector2.Transform(point - origin, System.Numerics.Matrix4x4.CreateRotationZ(rotation / (180 / MathF.PI))) + origin;
    }

}

[Serializable]
public struct Vector2I
{

    public int x { get; set; }
    public int y { get; set; }

    public Vector2I(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public float Magnitude()
    {
        return (float)Math.Sqrt(x * x + y * y);
    }

    public void Normalize()
    {
        float magnitude = Magnitude();
        if (magnitude > 0)
        {
            x = (int)MathF.Round(x / magnitude);
            y = (int)MathF.Round(y / magnitude);
        }
    }

    public static Vector2I operator +(Vector2I a, Vector2I b)
    {
        return new Vector2I(a.x + b.x, a.y + b.y);
    }

    public static Vector2I operator -(Vector2I a, Vector2I b)
    {
        return new Vector2I(a.x - b.x, a.y - b.y);
    }

    public static Vector2I operator *(Vector2I a, float scalar)
    {
        return new Vector2I((int)MathF.Round(a.x * scalar), (int)MathF.Round(a.y * scalar));
    }

    public static Vector2I operator *(Vector2I a, Vector2I scalar)
    {
        return new Vector2I(a.x * scalar.x, a.y * scalar.y);
    }

    public static Vector2I operator /(Vector2I a, float scalar)
    {
        if (scalar != 0)
        {
            return new Vector2I((int)MathF.Round(a.x / scalar), (int)MathF.Round(a.y / scalar));
        }
        throw new DivideByZeroException("Division by zero is not allowed.");
    }

    public static Vector2I operator /(Vector2I a, Vector2I scalar)
    {

        return new Vector2I((int)MathF.Round(a.x / scalar.x), (int)MathF.Round(a.y / scalar.y));

    }

    public static implicit operator System.Numerics.Vector2(Vector2I customVector)
    {
        return new System.Numerics.Vector2(customVector.x, customVector.y);
    }

    public static implicit operator Vector2I(System.Numerics.Vector2 customVector)
    {
        return new Vector2I((int)MathF.Round(customVector.X), (int)MathF.Round(customVector.Y));
    }
    public static implicit operator Vector2I(Vector2 customVector)
    {
        return new Vector2I((int)MathF.Round(customVector.x), (int)MathF.Round(customVector.y));
    }
    public static implicit operator Vector2(Vector2I customVector)
    {
        return new Vector2(customVector.x, customVector.y);
    }

    public override string ToString()
    {
        return $"({x}, {y})";
    }
    
}
