namespace VortexEngine;

public abstract class Collider : Component
{

    public override void Start()
    {
        if (GetComponent<Rigidbody>() == null){
            body.AddComponent<Rigidbody>().Static = true;
        }
    }

}
