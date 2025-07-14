namespace VortexEngine.Rendering;

public class Renderer
{
    public static IRenderer backend;
    public static IImguiBackend guiBackend;
    
    private static List<Vector2> PositionOffsets = new List<Vector2>();
    private static List<Vector2> SizeOffsets = new List<Vector2>();
    private static List<float> RotationOffsets = new List<float>();


    public static Vector2I TargetSize;

    public static Vector2I TargetCameraOveridePosition;

    public static RenderTexture UITarget = RenderTexture.Null;

    public static bool ShowUI = true;
    
    public static void DrawRectangle(Vector2 position, Vector2 size, float rotation, Color tint, bool ignoreOffsets = false)
    {

        Performance.PushTask("Rectangle");
        position = position * VortexEngine.UNIT_SCALE;
        size = size * VortexEngine.UNIT_SCALE;

        Vector2 scale = new Vector2(1, 1);
        
        if (!ignoreOffsets)
        {

            position = OffsetPosition(position);
        }
        
        backend.DrawRectanglePro(new Rect(position.x, position.y, size.x * scale.x, size.y * scale.y), size * scale / 2, rotation, tint);
        Performance.PopTask();
    }

    public static void DrawCircle(Vector2 position, Vector2 size, Color tint, bool ignoreOffsets = false)
    {
        Performance.PushTask("Circle");
        position = position * VortexEngine.UNIT_SCALE;
        size = size * VortexEngine.UNIT_SCALE;
        Vector2 scale = new Vector2(1, 1);

        if (!ignoreOffsets)
        {

            position = OffsetPosition(position);
        }

        backend.DrawEllipse(new Vector2(position.x, position.y),new Vector2((size * scale).x, (size * scale).y), tint);
        Performance.PopTask();
    }

    public static void DrawEdgeCircle(Vector2 position, float Radius, float rotation, Color tint, bool ignoreOffsets = false)
    {
        Performance.PushTask("Edge Circle");
        position = position * VortexEngine.UNIT_SCALE;
        Radius = Radius * VortexEngine.UNIT_SCALE;
        if (!ignoreOffsets)
        {

            position = OffsetPosition(position);
        }

        backend.DrawPolyLinesEx(position, 20, Radius, rotation, Debug.ColliderEdgeWidth *  VortexEngine.UNIT_SCALE, tint);
        Performance.PopTask();
    }

    public static void DrawEdgeRectangle(Vector2 position, Vector2 size, float rotation, Color tint, bool ignoreOffsets = false)
    {
        Performance.PushTask("Edge Rectangle");
        Vector2 scale = new Vector2(1, 1);

        position = position * VortexEngine.UNIT_SCALE;
        size = size * VortexEngine.UNIT_SCALE;
        
        if (!ignoreOffsets)
        {

            position = OffsetPosition(position);
        }

        Vector2 TopLeft = Vector2.RotateAboutOrigin(position + (size / 2) * new Vector2(-1, 1), position, rotation);
        Vector2 TopRight = Vector2.RotateAboutOrigin(position + (size / 2) * new Vector2(1, 1), position, rotation);
        Vector2 BottomLeft = Vector2.RotateAboutOrigin(position + (size / 2) * new Vector2(-1, -1), position, rotation);
        Vector2 BottomRight = Vector2.RotateAboutOrigin(position + (size / 2) * new Vector2(1, -1), position, rotation);
        
        

        backend.DrawLineEx(TopLeft, TopRight, Debug.ColliderEdgeWidth *  VortexEngine.UNIT_SCALE, tint);
        backend.DrawLineEx(TopRight, BottomRight, Debug.ColliderEdgeWidth *  VortexEngine.UNIT_SCALE, tint);
        backend.DrawLineEx(BottomRight, BottomLeft, Debug.ColliderEdgeWidth *  VortexEngine.UNIT_SCALE, tint);
        backend.DrawLineEx(BottomLeft, TopLeft, Debug.ColliderEdgeWidth *  VortexEngine.UNIT_SCALE, tint);
        
        backend.DrawCircleV(TopLeft, Debug.ColliderEdgeWidth *  VortexEngine.UNIT_SCALE / 2, tint);
        backend.DrawCircleV(TopRight, Debug.ColliderEdgeWidth *  VortexEngine.UNIT_SCALE / 2, tint);
        backend.DrawCircleV(BottomRight, Debug.ColliderEdgeWidth *  VortexEngine.UNIT_SCALE / 2, tint);
        backend.DrawCircleV(BottomLeft, Debug.ColliderEdgeWidth *  VortexEngine.UNIT_SCALE / 2, tint);
        

        Performance.PopTask();
    }

    public static void DrawArrow(Vector2 from, Vector2 to, Color tint, float rotation = 0){
        Performance.PushTask("Draw Arrow");
        to = from + new Vector2(0, 50);
        to = Vector2.RotateAboutOrigin(to, from, rotation);
        Renderer.DrawLine(from, to, tint);
        Vector2 tipL = to - new Vector2(10, 10);
        Vector2 tipR = to - new Vector2(-10, 10);
        tipL = Vector2.RotateAboutOrigin(tipL, to, rotation);
        tipR = Vector2.RotateAboutOrigin(tipR, to, rotation);
        Renderer.DrawLine(to, tipL, tint);
        Renderer.DrawLine(to, tipR, tint);
        Performance.PopTask();
    }

    public static void DrawLine(Vector2 a, Vector2 b, Color tint, bool ignoreOffsets = false){

        Performance.PushTask("Draw Line");
        a = a * VortexEngine.UNIT_SCALE;
        b = b * VortexEngine.UNIT_SCALE;


        if (!ignoreOffsets)
        {

            a = OffsetPosition(a);
            b = OffsetPosition(b);
        }
        backend.DrawLineEx(a, b, Debug.ColliderEdgeWidth *  VortexEngine.UNIT_SCALE, tint);
        Performance.PopTask();
    }



    public static void DrawTexture(Texture texture, Vector2 position, Vector2 size, float rotation, Color tint, bool ignoreOffsets = false)
    {
        Performance.PushTask("Draw Texture");
        position = position * VortexEngine.UNIT_SCALE;
        size = size * VortexEngine.UNIT_SCALE;

        if (!ignoreOffsets)
        {
            position = OffsetPosition(position);
        }
        
        backend.DrawTexturePro(texture, new Rect(0, 0, texture.Width, texture.Height), new Rect(position.x, position.y, size.x, size.y), size / 2, rotation, tint);
        Performance.PopTask();
    }

    public static void DrawTexture(Texture texture, Vector2 position, Vector2 size, Rect region, float rotation, Color tint, bool ignoreOffsets = false)
    {
        Performance.PushTask("Draw Texture");
        position = position * VortexEngine.UNIT_SCALE;
        size = size * VortexEngine.UNIT_SCALE;

        if (!ignoreOffsets)
        {
            position = OffsetPosition(position);
        }


        Vector2 textureSize = new Vector2(texture.Width, texture.Height);
        backend.DrawTexturePro(texture, new Rect(region.Position.x, region.Position.y, region.Size.x, region.Size.y), new Rect(position.x, position.y, size.x, size.y), size / 2, rotation, tint);
        Performance.PopTask();
    }

    public static void DrawTexture(Texture texture, Vector2 position, Vector2 size, float rotation, bool ignoreOffsets = false) => DrawTexture(texture, position, size, rotation, Color.White, ignoreOffsets);
    public static void DrawTexture(Texture texture, Vector2 position, Vector2 size, bool ignoreOffsets = false) => DrawTexture(texture, position, size, 0, Color.White, ignoreOffsets);
    public static void DrawTexture(Texture texture, Vector2 position, bool ignoreOffsets = false) => DrawTexture(texture, position, Vector2.One, 0, Color.White, ignoreOffsets);

    private static Vector2 OffsetPosition(Vector2 position)
    {

        foreach (Vector2 pos in PositionOffsets)
        {
            position += pos;
        }

        return position;

    }

    private static Vector2 OffsetSize(Vector2 size)
    {

        foreach (Vector2 sizeOffset in SizeOffsets)
        {
            size *= sizeOffset;
        }

        return size;

    }

    private static float OffsetRotation(float rotation)
    {

        foreach (float rot in RotationOffsets)
        {
            rotation += rot;
        }

        return rotation;

    }

    public static RenderTexture target = RenderTexture.Null;



    public static void BeginFrame(RenderTexture target){
        
        Renderer.target = target;
        TargetSize = new Vector2(target.Width, target.Height);


    }

    public static void EndFrame()
    {
        if (UITarget.id != 0 && ShowUI){
            Vector2I offsetOverride = OffsetPosition(TargetCameraOveridePosition) - new Vector2(TargetSize.x / 2, TargetSize.y / 2);
            backend.DrawRenderTexturePro(UITarget, new Rect(0, 0, UITarget.Width, UITarget.Height), new Rect(offsetOverride, TargetSize.x, TargetSize.y), Vector2.Zero, 0.0f, Color.White);

            backend.DrawRectangleLines(new Vector2(offsetOverride.x, offsetOverride.y), new Vector2(TargetSize.x, TargetSize.y), Color.Yellow);

        }


        PositionOffsets.Clear();
        RotationOffsets.Clear();
        SizeOffsets.Clear();
    }

    public static void AddPositionOffset(Vector2 offset)
    {

        PositionOffsets.Add(offset);
    }

    public static void AddSizeOffset(Vector2 offset)
    {
        SizeOffsets.Add(offset);
    }

    public static void AddRotationOffset(float rotation)
    {
        RotationOffsets.Add(rotation);
    }
    
    public static void BeginUIMode(){
        Performance.PushTask("UI Mode");
        Performance.PushTask("Begin UI Mode");
        
        if (UITarget.id == -1){
            Performance.PushTask("Create UI RenderTexture");
            UITarget = backend.LoadRenderTexture(TargetSize);
            Performance.PopTask();
        }
        backend.EndTextureMode();
        backend.BeginTextureMode(UITarget);
        backend.ClearBackground(Color.Clear);
        Performance.PopTask();
    }
    public static void EndUIMode(){
        Performance.PushTask("End UI Mode");
        backend.EndTextureMode();
        backend.BeginTextureMode(target);
        Performance.PopTask();
        Performance.PopTask();
    }
    
    public static void DrawText(string text, Vector2 position, int fontSize, Color color, float rotation = 0, bool ignoreOffsets = false)
    {
        
        Performance.PushTask("Text Rendering");

        if (fontSize <= 0){
            return;
        }
        
        backend.DrawTextEx(text, position, fontSize, 5.0f, color);
        
        Performance.PopTask();

    }

}
