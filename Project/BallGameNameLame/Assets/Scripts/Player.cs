using System.Linq.Expressions;
using System.Text.Json.Serialization;
using VortexEngine;

public class Player : Component
{

    
    
    public Body scoreText;
    public int score = 0;

    public float KillPlane = -100f;

    //[JsonIgnore, HideInInspector]
    public Rigidbody rb;


    public Prefab RedballPrefab;

    public float SpawnSpeed = 0.5f;
    public float YellowSpawnSpeed = 1f;
    public float SpawnRange = 205;

    [JsonIgnore]
    Random yellowRand;
    [JsonIgnore]
    Random redRand;

    public override void Start()
    {




        GetComponent<Rigidbody>().OnCollisionCallback += OnCollision;

        rb = GetComponent<Rigidbody>();

        Invoke(SpawnRed, SpawnSpeed);
        Invoke(SpawnYellow, YellowSpawnSpeed);

        yellowRand = new Random(Random.Shared.Next(0, 1000));
        redRand = new Random(yellowRand.Next(0, 1000));

    }

    public override void DrawDebugGui()
    {
        
    }

    public override void Update()
    {

        if (transform.Position.y < KillPlane){
            Kill();
        }

        rb.velocity = new Vector2(-Input.GetAxis(KeyCode.LEFT, KeyCode.RIGHT), rb.velocity.y);

        scoreText.GetComponent<TextRenderer>().Text = "Points: " + score.ToString();


    }


    void SpawnRed(){
        Body _body = VortexEngine.VortexEngine.Master.AddSceneAsPrefab("ball_red" + ".vobj" ,new Vector2((float)(redRand.NextDouble() * (SpawnRange * 2) - SpawnRange/2), 100));

        Invoke(SpawnRed, SpawnSpeed);
    }

    void SpawnYellow(){
        Body __body = VortexEngine.VortexEngine.Master.AddSceneAsPrefab("ball_yellow" + ".vobj", new Vector2((float)(yellowRand.NextDouble() * (SpawnRange * 2) - SpawnRange/2), 100));

        Invoke(SpawnYellow, YellowSpawnSpeed);
    }

    void OnCollision(Rigidbody other){

    }

    public void Kill(){
        Console.WriteLine("Game Over");
        GetComponent<Rigidbody>().position = new Vector2(0, 0);
        VortexEngine.VortexEngine.Master.LoadScene("scene.vscn");
    }


}
