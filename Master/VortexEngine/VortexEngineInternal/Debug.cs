namespace VortexEngine;

public static class Debug{

    public static bool DrawColliders = false;
    public static bool DrawBodys = true;
    public static float ColliderEdgeWidth = 1;

    public static List<string> log = new List<string>();

    public static void Log(string str){
        log.Insert(0, str);
    }

}
