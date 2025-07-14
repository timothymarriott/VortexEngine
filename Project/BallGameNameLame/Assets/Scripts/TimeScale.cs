namespace VortexEngine.Sample;

public class TimeScale : Component
{

    public float tscale = 1;
    
    public override void Update()
    {
        Time.TimeScale = tscale;
    }
}