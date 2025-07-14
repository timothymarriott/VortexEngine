namespace VortexEngine;

[Serializable]
public class Prefab
{

    public string FileHandle = "";

    public static Prefab GetFromHandle(string file){
        return new Prefab(file);
    }

    public Prefab(string filehandle){
        FileHandle = filehandle;
    }

    public Body Create(){

        Body? body = VortexEngine.Master.AddSceneAsPrefab(FileHandle + ".vobj", Vector2.Zero);
        if (body != null){
            return body;
        } else {
            throw new Exception("Prefab is empty");
        }

    }

}
