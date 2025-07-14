using Raylib_cs;

namespace VortexEngine.Editor;

public static class Icons
{

    private static Dictionary<string, Texture2D> icons = new Dictionary<string, Texture2D>();

    public static void LoadIcons()
    {
        Console.WriteLine("Loading Icons from " + VortexEngine.EditorDataPath + "/Icons/");
        icons.Add("File", Raylib.LoadTexture(VortexEngine.EditorDataPath + "/Icons/Icon_File.png"));
        icons.Add("Folder", Raylib.LoadTexture(VortexEngine.EditorDataPath + "/Icons/Icon_Folder.png"));
        icons.Add("Image", Raylib.LoadTexture(VortexEngine.EditorDataPath + "/Icons/Icon_Image.png"));
        icons.Add("Arrow", Raylib.LoadTexture(VortexEngine.EditorDataPath + "/Icons/Arrow.png"));
    }

    public static Texture2D Get(string icon)
    {
        return icons[icon];
    }

}
