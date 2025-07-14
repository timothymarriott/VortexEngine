using VortexEngine.Rendering;

namespace VortexEngine;

public class BoxCollider : Collider
{

    
    public Vector2 Size
    {
        get => _size;
        set {
            _size = value;
            if (GetComponent<Rigidbody>() != null){
                GetComponent<Rigidbody>().SetCollider(this);
            }
        }
    }

    public Vector2 _size;

    public override void DrawGizmos()
    {
        if (Debug.DrawColliders){
            Renderer.DrawEdgeRectangle(transform.Position, Size, transform.Rotation, Color.FromGuid(body.ID) * new Color(255, 255, 255, 255));
        }
    }


}
