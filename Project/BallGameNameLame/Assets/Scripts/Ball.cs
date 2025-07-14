using System.Text.Json.Serialization;
using VortexEngine;

public class Ball : Component{

    
    private Rigidbody rb;

    public bool IsPoint;


    public override void Start()
    {

        //AddComponent<SpriteTexture>().TextureID = "Sprite_Background";
        //transform.Scale = new Vector2(144, 256);
        rb = GetComponent<Rigidbody>();

        transform.Position.y = 100;

        rb.OnCollisionCallback += OnCollision;

    }

    private void OnCollision(Rigidbody rigidbody)
    {
        if (IsPoint && rigidbody.GetComponent<Player>() != null){
            rigidbody.GetComponent<Player>().score++;
            Destroy(body);
        }
    }

    public override void Update()
    {
        if (transform.Position.y < -100){
            Destroy(body);
        }

    }

}
