using System.Text.Json.Serialization;
using VortexEngine;
using VortexEngine.Internal.AssetManagement;

[Serializable]
public class AnimationClip{

    public AnimationFrame[] Frames = new AnimationFrame[0];
    public bool Loop = true;
    public float FPS = 1;

    public static AnimationClip? Load(string id){
        AnimationClip? clip = AssetManager.LoadData<AnimationClip>(id);
        if (clip == null){
            Console.WriteLine("ERROR: No animation with id \"{id}\"");
            return null;
        }
        return clip;
    }

    public void Save(string id){
        AssetManager.SaveData(this, id);
    }

}

[Serializable]
public class AnimationFrame{

    public string TextureID;

    public int FrameDuration;

    public bool UseRegion;
    public Rect Region;

    public Vector2 Offset;

    [JsonConstructor]
    public AnimationFrame(string TextureID, int FrameDuration, bool UseRegion, Rect Region, Vector2 Offset){
        this.TextureID = TextureID;
        this.FrameDuration = FrameDuration;
        this.UseRegion = UseRegion;
        this.Region = Region;
        this.Offset = Offset;
    }

    public AnimationFrame(string TextureID){
        this.TextureID = TextureID;
        this.FrameDuration = 1;
        this.UseRegion = false;
        this.Region = Rect.Zero;
        this.Offset = Vector2.Zero;
    }

}