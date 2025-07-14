using System.Text.Json.Serialization;
using ImGuiNET;

namespace VortexEngine;

public class Transform : Component
{

    public Vector2 Position;
    public Vector2 Scale = Vector2.One;
    public float Rotation;

    [HideInInspector]
    public Body Parent;

    [JsonIgnore]
    public List<Body> Children {
        get{
            List<Body> res = new List<Body>();
            foreach (var body in VortexEngine.Master.LoadedScene.Bodys)
            {
                if (body.transform.Parent == this.body){
                    res.Add(body);
                }
            }
            return res;
        }

    }
    
    public override void OnValidate(){
        if (Parent == body){
            Parent = null;
        }
    }

    public Vector2 GetVisualScale(){
        if (GetComponent<SpriteTexture>() != null){
            if (GetComponent<SpriteTexture>().Region.Size.Magnitude() > 0)
                return GetComponent<SpriteTexture>().Region.Size;
            else return GetComponent<SpriteTexture>().TextureSize;
        }
        if (GetComponent<Camera>() != null){
            return Vector2.Zero;
        }

        if (GetComponent<CircleCollider>() != null)
        {
            return new Vector2(GetComponent<CircleCollider>()._radius, GetComponent<CircleCollider>()._radius) * 2;
        }
        
        return Scale;
    }

    public Rect GetVisualRect(){
        Rect res = new Rect();
        res.Size = GetVisualScale();
        res.Position = Position - res.Size/2;
        return res;
    }

}
