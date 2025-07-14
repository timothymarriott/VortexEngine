namespace VortexEngine;

[Serializable]
public struct Rect
{

    public readonly static Rect Zero = new Rect(new(0, 0), new(0, 0));

    public Vector2 Position;

    public Vector2 Size;

    public float Rotation = 0;

    public Rect(Vector2 position, Vector2 size, float rotation = 0){
        Position = position;
        Size = size;
        Rotation = rotation;

    }
    
    public Rect(float posX, float posY, float sizeX, float sizeY, float rotation = 0){
        Position = new Vector2(posX, posY);
        Size = new Vector2(sizeX, sizeY);
        Rotation = rotation;
    }
    
    public Rect(float posX, float posY, Vector2 size, float rotation = 0){
        Position = new Vector2(posX, posY);
        Size = size;
        Rotation = rotation;
    }
    
    public Rect(Vector2 pos, float sizeX, float sizeY, float rotation = 0){
        Position = pos;
        Size = new Vector2(sizeX, sizeY);
        Rotation = rotation;
    }

    public bool Contains(Vector2 point)
    {
        Vector2 unrotatedPoint = Vector2.RotateAboutOrigin(point, Position, -Rotation);

        unrotatedPoint += Size/2;

        return unrotatedPoint.x >= Position.x &&
               unrotatedPoint.x <= Position.x + Size.x &&
               unrotatedPoint.y >= Position.y &&
               unrotatedPoint.y <= Position.y + Size.y;
    }

}
