using VortexEngine.Rendering;

namespace VortexEngine;

public class CircleCollider : Collider
{
    public float Radius{
        get => _radius;
        set {
            _radius = value;
            if (GetComponent<Rigidbody>() != null){
                GetComponent<Rigidbody>().SetCollider(this);
            }
        }
    }

    public float _radius;

    public override void DrawGizmos()
    {
        if (Debug.DrawColliders){

            Renderer.DrawEdgeCircle(transform.Position, Radius, transform.Rotation, Color.FromGuid(body.ID) * new Color(255, 255, 255, 255));

        }
    }

}
