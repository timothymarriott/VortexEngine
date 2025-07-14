namespace VortexEngine;

public class InducedLag : Component
{
    public int level;

    public override void Update()
    {
        Performance.PushTask("Induced Lag");
        for (int i = 0; i < level; i++)
        {
            VortexEngine.Master.LoadedScene.FindObjectOfType<Transform>();
        }
        Performance.PopTask();
    }

}
