using Genbox.VelcroPhysics.Collision.ContactSystem;
using Genbox.VelcroPhysics.Dynamics;
using Genbox.VelcroPhysics.Factories;
using Genbox.VelcroPhysics.Utilities;
using System.Text.Json.Serialization;

namespace VortexEngine;

public class Rigidbody : Component
{
    [JsonIgnore]
    public Genbox.VelcroPhysics.Dynamics.Body physicsBody;

    public bool Static = false;

    public float GravityScale = 1f;

    public bool FreezeRotation = false;

    public float Mass = 1;

    [JsonIgnore]
    public Action<Rigidbody> OnCollisionCallback;

    [JsonIgnore]
    public Vector2 position
    {
        get => ConvertUnits.ToDisplayUnits(physicsBody.Position);
        set {
            if (physicsBody != null){
                physicsBody.Position = ConvertUnits.ToSimUnits(value);
            }
        }
    }


    [JsonIgnore]
    public float rotation
    {
        get => (180 / MathF.PI) *  transform.Rotation;
        set {
            if (physicsBody != null){
                physicsBody.Rotation = (MathF.PI / 180) * value;
            }
        }
    }

    [JsonIgnore]
    public Vector2 velocity
    {
        get => physicsBody.LinearVelocity;
        set {
            if (physicsBody != null){
                physicsBody.LinearVelocity = value;
            }
        }
    }

    public void SetCollider(Collider collider)
    {

        try
        {
            if (physicsBody != null)
            {
                physicsBody.RemoveFromWorld();
            }
        }
        catch
        {
            Log.Warning("Error when removing from world the old collider.");
        }
        
        if (collider == null){

            physicsBody = BodyFactory.CreateBody(VortexEngine.Master.LoadedScene.PhysicsWorld, ConvertUnits.ToSimUnits(transform.Position), transform.Rotation, Genbox.VelcroPhysics.Dynamics.BodyType.Dynamic);

        } else if (collider is CircleCollider c)
        {
            physicsBody = BodyFactory.CreateCircle(VortexEngine.Master.LoadedScene.PhysicsWorld, ConvertUnits.ToSimUnits(c.Radius), 1f, ConvertUnits.ToSimUnits(transform.Position), Genbox.VelcroPhysics.Dynamics.BodyType.Dynamic);

        } else if (collider is BoxCollider)
        {
            physicsBody = BodyFactory.CreateRectangle(VortexEngine.Master.LoadedScene.PhysicsWorld, ConvertUnits.ToSimUnits((collider as BoxCollider).Size.x), ConvertUnits.ToSimUnits((collider as BoxCollider).Size.y), 1f, ConvertUnits.ToSimUnits(transform.Position), transform.Rotation, Genbox.VelcroPhysics.Dynamics.BodyType.Dynamic);
        }

        physicsBody.Mass *= Mass;

        if (Static)
        {
            physicsBody.BodyType = BodyType.Static;
            physicsBody.LinearVelocity = Vector2.Zero;
        }

        physicsBody.OnCollision += OnCollision;
        
    }

    internal void OnCollision(Fixture fixtureA, Fixture fixtureB, Contact contact){
        
        Genbox.VelcroPhysics.Dynamics.Body? bodyA = null;
        Genbox.VelcroPhysics.Dynamics.Body? bodyB = null;
        VortexEngine.Master.LoadedScene.PhysicsWorld.BodyList.ForEach(b => {
            if (fixtureA.Body == b) bodyA = fixtureA.Body;
            if (fixtureB.Body == b) bodyB = fixtureB.Body;
        });

        Rigidbody? rbA = null;
        Rigidbody? rbB = null;

        VortexEngine.Master.LoadedScene.Bodys.ForEach(b => {
            if (b.GetComponent<Rigidbody>() != null){
                if (b.GetComponent<Rigidbody>().physicsBody == bodyA) rbA = b.GetComponent<Rigidbody>();
                if (b.GetComponent<Rigidbody>().physicsBody == bodyB) rbB = b.GetComponent<Rigidbody>();
            }

        });
        
        if (OnCollisionCallback != null && rbA != null && rbB != null){
            if (rbA != this) OnCollisionCallback(rbA);
            if (rbB != this) OnCollisionCallback(rbB);
        }
        
    }

    public void AddForce(Vector2 force, bool impulse = true)
    {
        if (!impulse)
            physicsBody.ApplyForce(force, position);
        else
            physicsBody.ApplyLinearImpulse(force);
    }

    public void ApplyTorque(float torgue)
    {
        physicsBody.ApplyTorque(torgue);
    }

    public override void Awake(){
        SetCollider(GetComponent<Collider>());
    }

    public override void Start()
    {
        
    }



    public override void OnDestroyed()
    {

        try
        {
            physicsBody.Enabled = false;

            physicsBody.RemoveFromWorld();
        }
        catch
        {
            Log.Error("Failed to remove physics body, may leave lingering hitboxes.");
        }

    }

    public override void Update()
    {

        if (physicsBody == null)
        {
            Log.Warning("Null Physics body.");
            SetCollider(GetComponent<Collider>());
        }
        
        if (Static)
        {
            physicsBody.BodyType = BodyType.Static;
            velocity = Vector2.Zero;
        } else {
            physicsBody.GravityScale = GravityScale;
            physicsBody.FixedRotation = FreezeRotation;
            
            transform.Position = ConvertUnits.ToDisplayUnits(physicsBody.Position);
            transform.Rotation = (180 / MathF.PI) * physicsBody.Rotation;
        }
        
    }
}
