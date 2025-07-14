namespace VortexEngine.Internal;

public static class Extensions
{

    public static List<Component.ComponentData> GetData(this List<Component> components)
    {
        List<Component.ComponentData> result = new List<Component.ComponentData>();

        foreach (Component comp in components)
        {
            result.Add(comp.GetData());
        }

        return result;
    }

}
