using VortexEngine;

public class Animator : Component{

    float CurrentClipTime;

    AnimationClip? RunningClip;

    int CurrentFrame;

    public override void Start()
    {

    }

    public override void Update()
    {

        if (RunningClip == null) return;

        CurrentClipTime += Time.DeltaTime;

        if (Time.Completed(body.ID + "_frame_" + CurrentFrame)){
            CurrentFrame++;

            if (CurrentFrame == RunningClip.Frames.Length){
                if (RunningClip.Loop){
                    CurrentFrame = 0;
                    Time.AddTimer(body.ID + "_frame_" + CurrentFrame, 1 / RunningClip.FPS * RunningClip.Frames[0].FrameDuration);
                } else {
                    CurrentClipTime = 0;
                    CurrentFrame = 0;
                    RunningClip = null;
                    return;
                }
            } else {
                Time.AddTimer(body.ID + "_frame_" + CurrentFrame, 1 / RunningClip.FPS * RunningClip.Frames[0].FrameDuration);
            }

            SpriteTexture? renderer = GetComponent<SpriteTexture>();
            if (renderer != null){
                renderer.TextureID = RunningClip.Frames[CurrentFrame].TextureID;
                if (RunningClip.Frames[CurrentFrame].UseRegion)
                    renderer.Region = RunningClip.Frames[CurrentFrame].Region;

                transform.Position += RunningClip.Frames[CurrentFrame].Offset;

            }

        }

    }

    public void PlayClip(AnimationClip clip){
        if (Time.Timers.ContainsKey(body.ID + "_frame_" + CurrentFrame))
            Time.Timers.Remove(body.ID + "_frame_" + CurrentFrame);
        CurrentClipTime = 0;
        CurrentFrame = 0;
        RunningClip = clip;
        Time.AddTimer(body.ID + "_frame_" + CurrentFrame, 1 / clip.FPS * clip.Frames[0].FrameDuration);

        SpriteTexture? renderer = GetComponent<SpriteTexture>();
        if (renderer != null){
            renderer.TextureID = RunningClip.Frames[CurrentFrame].TextureID;
            if (RunningClip.Frames[CurrentFrame].UseRegion)
                renderer.Region = RunningClip.Frames[CurrentFrame].Region;

            transform.Position += RunningClip.Frames[CurrentFrame].Offset;

        }
    }

}
