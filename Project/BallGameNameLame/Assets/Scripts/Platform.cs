using System.Text.Json.Serialization;
using VortexEngine;

public class Platform : Component{

    [JsonIgnore]
    private Rigidbody rb;

    public override void Start()
    {

        //AddComponent<SpriteTexture>().TextureID = "Sprite_Background";
        //transform.Scale = new Vector2(144, 256);
        rb = GetComponent<Rigidbody>();
    }

    public override void Update()
    {
        //rb.position = Vector2.Zero;
        if ((Vector2.Zero - rb.position).Magnitude() < 2){
            rb.velocity = (Vector2.Zero - rb.position);
        } else {
            rb.position = new Vector2(0, 0);
        }




    }

}
