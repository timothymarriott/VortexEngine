using Genbox.VelcroPhysics.Dynamics;

namespace VortexEngine;

public static class Physics
{

    public static List<Body> Raycast(Vector2 Start, Vector2 End){
        List<Body> result = new List<Body>();
        List<Fixture> fixtures = VortexEngine.Master.LoadedScene.PhysicsWorld.RayCast(Start, End);
        for (int i = 0; i < fixtures.Count; i++)
        {
            result.Add(GetBodyOfPhysicsBody(fixtures[i].Body)); 
        }
        return result;
    }

    public static Body? GetBodyOfPhysicsBody(Genbox.VelcroPhysics.Dynamics.Body body){
        foreach (var item in VortexEngine.Master.LoadedScene.Bodys)
        {
            if (item.GetComponent<Rigidbody>() != null){
                if (item.GetComponent<Rigidbody>().physicsBody == body){
                    return item;
                }
            }
        }
        return null;
    }

}
