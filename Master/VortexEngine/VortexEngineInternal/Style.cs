using System.Numerics;
using ImGuiNET;
using VortexEngine.Rendering;

namespace VortexEngine.Editor;

public class Theme{
    public ImGuiCol MainColor = ImGuiCol.Button;
    public Dictionary<ImGuiCol, System.Numerics.Vector4> Colors = new Dictionary<ImGuiCol, System.Numerics.Vector4>();
    public float FrameRounding = 4;
    public string fontPath = "";
    public float FontSize = 15f;
    
    public Theme(System.Numerics.Vector4 targetColor) : this(){
        this.Colors = Style.GetDefaultTheme().Colors;
        this.MainColor = Style.GetDefaultTheme().MainColor;
        Vector4 baseColor = Colors[MainColor];

        float hueShift = Style.CalculateHueShift(baseColor, targetColor);

        foreach (var key in Colors.Keys.ToList())
        {
            Colors[key] = Style.ShiftHue(Colors[key], hueShift);
        }

        Colors[MainColor] = targetColor;

    }

    public Theme()
    {

        fontPath = VortexEngine.EditorDataPath + "font.ttf";
    }
}

public static class Style
{



    public static System.Numerics.Vector4 ShiftHue(System.Numerics.Vector4 color, float hueShift)
    {
        (float h, float s, float v) = RgbaToHsva(color);

        h = (h + hueShift) % 1f;
        if (h < 0) h += 1f;

        System.Numerics.Vector3 rgb = HsvaToRgba(h, s, v);

        return new System.Numerics.Vector4(rgb, color.W);
    }

    private static (float h, float s, float v) RgbaToHsva(System.Numerics.Vector4 color)
    {
        float r = color.X, g = color.Y, b = color.Z;

        float max = MathF.Max(r, MathF.Max(g, b));
        float min = MathF.Min(r, MathF.Min(g, b));
        float delta = max - min;

        float h = 0f;
        if (delta > 0)
        {
            if (max == r)
                h = (g - b) / delta % 6;
            else if (max == g)
                h = (b - r) / delta + 2;
            else
                h = (r - g) / delta + 4;
            h /= 6f;
            if (h < 0) h += 1f;
        }

        float s = max == 0 ? 0 : delta / max;
        float v = max;

        return (h, s, v);
    }

    private static System.Numerics.Vector4 AdjustColor(System.Numerics.Vector4 color, float hueShift, float newAlpha)
    {
        (float h, float s, float v) = RgbaToHsva(color);

        h = (h + hueShift) % 1f;
        if (h < 0) h += 1f;

        System.Numerics.Vector3 rgb = HsvaToRgba(h, s, v);
        return new System.Numerics.Vector4(rgb, newAlpha);
    }


    private static System.Numerics.Vector3 HsvaToRgba(float h, float s, float v)
    {
        float c = v * s;
        float x = c * (1 - MathF.Abs((h * 6) % 2 - 1));
        float m = v - c;

        float r = 0, g = 0, b = 0;

        if (h < 1f / 6f)      { r = c; g = x; }
        else if (h < 2f / 6f) { r = x; g = c; }
        else if (h < 3f / 6f) { g = c; b = x; }
        else if (h < 4f / 6f) { g = x; b = c; }
        else if (h < 5f / 6f) { r = x; b = c; }
        else                  { r = c; b = x; }

        return new System.Numerics.Vector3(r + m, g + m, b + m);
    }


    public static float CalculateHueShift(Vector4 fromColor, Vector4 toColor)
    {
        (float h1, _, _) = RgbaToHsva(fromColor);
        (float h2, _, _) = RgbaToHsva(toColor);
        float hueShift = h2 - h1;

        if (hueShift < -0.5f) hueShift += 1f;
        if (hueShift > 0.5f) hueShift -= 1f;

        return hueShift;
    }


    public static void ApplyCustomTheme(System.Numerics.Vector4 targetColor){


        Theme theme = GetDefaultTheme();
        Vector4 baseColor = theme.Colors[theme.MainColor];

        float hueShift = CalculateHueShift(baseColor, targetColor);

        foreach (var key in theme.Colors.Keys.ToList())
        {
            theme.Colors[key] = ShiftHue(theme.Colors[key], hueShift);
        }

        theme.Colors[theme.MainColor] = targetColor;

        ApplyTheme(theme);

    }

    public static bool FontLoaded = false;
    public static ImFontPtr font;
    public static Theme currentTheme;

    public static void Begin()
    {
        
        if (!FontLoaded){
            FontLoaded = true;

            unsafe {

                string fontPath = currentTheme.fontPath;
                if (!File.Exists(fontPath))
                {
                    Console.WriteLine("Font file not found at: " + fontPath);
                    throw new Exception("Font file not found at: " + fontPath);
                }

                ImGuiIOPtr io = ImGui.GetIO();
                ImFontConfigPtr configuration = ImGuiNative.ImFontConfig_ImFontConfig();

                configuration.OversampleH = 2;
                configuration.OversampleV = 2;

                configuration.MergeMode = false;

                font = io.Fonts.AddFontFromFileTTF(fontPath, currentTheme.FontSize, configuration);

                font.Scale = 1f;

                io.Fonts.Build();

                Renderer.guiBackend.ReloadFonts();
                
            }

        }
    }

    public static void End()
    {
        
        ImGui.PopFont();
    }

    public static void Shutdown()
    {
        
    }
    
    public static void ApplyTheme(Theme theme)
    {
        currentTheme = theme;
        ImGuiStylePtr style = ImGui.GetStyle();
        foreach (var color in theme.Colors){
            style.Colors[(int)color.Key] = color.Value;
        }

        style.FrameRounding = theme.FrameRounding;
    }


    private static Vector4 HueToRgb(float hue)
    {
        float r = 0, g = 0, b = 0;
        float c = 1.0f; 
        float x = c * (1 - MathF.Abs((hue * 6) % 2 - 1));
        float m = 0.0f;

        if (hue < 1f / 6f) { r = c; g = x; }
        else if (hue < 2f / 6f) { r = x; g = c; }
        else if (hue < 3f / 6f) { g = c; b = x; }
        else if (hue < 4f / 6f) { g = x; b = c; }
        else if (hue < 5f / 6f) { r = x; b = c; }
        else { r = c; b = x; }

        return new Vector4(r + m, g + m, b + m, 1.0f);
    }

    public static Theme GetDefaultTheme(){

        Theme theme = new Theme();
        theme.Colors = new(){
            {ImGuiCol.Border, new System.Numerics.Vector4(0.14f, 0.14f, 0.17f, 0.50f)},

            {ImGuiCol.ScrollbarGrab, new System.Numerics.Vector4(0.11f, 0.11f, 0.11f, 1.00f)},
            {ImGuiCol.ScrollbarGrabHovered, new System.Numerics.Vector4(0.08f, 0.08f, 0.08f, 1.00f)},
            {ImGuiCol.ScrollbarGrabActive, new System.Numerics.Vector4(0.07f, 0.07f, 0.07f, 1.00f)},

            {ImGuiCol.ButtonHovered, new System.Numerics.Vector4(0.28f, 0.26f, 0.98f, 1.00f)},

            {ImGuiCol.FrameBg, new System.Numerics.Vector4(0.17f, 0.16f, 0.48f, 0.54f)},
            {ImGuiCol.FrameBgHovered, new System.Numerics.Vector4(0.28f, 0.26f, 0.98f, 0.40f)},
            {ImGuiCol.FrameBgActive, new System.Numerics.Vector4(0.28f, 0.26f, 0.98f, 0.67f)},
            {ImGuiCol.TitleBgActive, new System.Numerics.Vector4(0.17f, 0.16f, 0.48f, 1.00f)},
            {ImGuiCol.CheckMark, new System.Numerics.Vector4(0.28f, 0.26f, 0.98f, 1.00f)},
            {ImGuiCol.SliderGrab, new System.Numerics.Vector4(0.26f, 0.24f, 0.88f, 1.00f)},
            {ImGuiCol.SliderGrabActive, new System.Numerics.Vector4(0.28f, 0.26f, 0.98f, 1.00f)},
            {ImGuiCol.Button, new System.Numerics.Vector4(0.28f, 0.26f, 0.98f, 0.40f)},
            {ImGuiCol.ButtonActive, new System.Numerics.Vector4(0.20f, 0.18f, 1.00f, 1.00f)},
            {ImGuiCol.Header, new System.Numerics.Vector4(0.28f, 0.26f, 0.98f, 0.31f)},
            {ImGuiCol.HeaderHovered, new System.Numerics.Vector4(0.28f, 0.26f, 0.98f, 0.80f)},
            {ImGuiCol.HeaderActive, new System.Numerics.Vector4(0.28f, 0.26f, 0.98f, 1.00f)},
            {ImGuiCol.SeparatorHovered, new System.Numerics.Vector4(0.12f, 0.10f, 0.75f, 0.78f)},
            {ImGuiCol.SeparatorActive, new System.Numerics.Vector4(0.12f, 0.10f, 0.75f, 1.00f)},
            {ImGuiCol.ResizeGrip, new System.Numerics.Vector4(0.28f, 0.26f, 0.98f, 0.20f)},
            {ImGuiCol.ResizeGripHovered, new System.Numerics.Vector4(0.28f, 0.26f, 0.98f, 0.67f)},
            {ImGuiCol.ResizeGripActive, new System.Numerics.Vector4(0.28f, 0.26f, 0.98f, 0.95f)},
            {ImGuiCol.TabHovered, new System.Numerics.Vector4(0.28f, 0.26f, 0.98f, 0.80f)},
            {ImGuiCol.Tab, new System.Numerics.Vector4(0.19f, 0.18f, 0.58f, 0.86f)},
            {ImGuiCol.TabSelected, new System.Numerics.Vector4(0.21f, 0.20f, 0.68f, 1.00f)},
            {ImGuiCol.TabSelectedOverline, new System.Numerics.Vector4(0.28f, 0.26f, 0.98f, 1.00f)},
            {ImGuiCol.TabDimmed, new System.Numerics.Vector4(0.07f, 0.07f, 0.15f, 0.97f)},
            {ImGuiCol.TabDimmedSelected, new System.Numerics.Vector4(0.14f, 0.14f, 0.42f, 1.00f)},
            {ImGuiCol.DockingPreview, new System.Numerics.Vector4(0.28f, 0.26f, 0.98f, 0.70f)},
            {ImGuiCol.DockingEmptyBg, new System.Numerics.Vector4(0.00f, 0.00f, 0.00f, 0.00f)},
            {ImGuiCol.TextSelectedBg, new System.Numerics.Vector4(0.28f, 0.26f, 0.98f, 0.35f)},
            {ImGuiCol.NavWindowingHighlight, new System.Numerics.Vector4(0.28f, 0.26f, 0.98f, 1.00f)},
        };

        return theme;

    }

}
