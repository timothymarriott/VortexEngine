using System.Numerics;
using ImGuiNET;
using Raylib_cs;
using rlImGui_cs;
using VortexEngine.Editor.Windows;
using VortexEngine.Rendering;
using VortexEngine.Rendering.Backends;

namespace VortexEngine.Editor;

public class SceneWindow : EditorWindow
{

    public Vector2 position;
    public Color BackgroundColor = new Color(73, 73, 73);
    public Vector2I Size = new Vector2(1920, 1080);
    public float Zoom = 2;
    public float ZoomSpeed = 0.1f;
    public RenderTexture renderTexture = RenderTexture.Null;

    public bool YArrowHovered = false;
    public bool YDraging = false;
    public bool XDraging = false;
    public bool XArrowHovered = false;
    public Vector2 XLastDragPos;
    public Vector2 YLastDragPos;

    public void Render()
    {

        if (VortexEngine.Master.LoadedScene == null) return;

        float oldScale = VortexEngine.UNIT_SCALE;

        VortexEngine.UNIT_SCALE = Zoom;
        
        Performance.PushTask("Setup");
        Renderer.ShowUI = false;
        if (renderTexture.Width != Size.x && renderTexture.Height != Size.y)
        {
            Renderer.backend.UnloadRenderTexture(renderTexture);
            renderTexture = Renderer.backend.LoadRenderTexture(new Vector2(Size.x, Size.y));
        }
        
        Renderer.backend.BeginTextureMode(renderTexture);

        Renderer.BeginFrame(renderTexture);
        if (VortexEngine.Master.LoadedScene != null)
            if (VortexEngine.Master.LoadedScene.FindObjectOfType<Camera>() != null){
                Renderer.TargetSize = VortexEngine.Master.LoadedScene.FindObjectOfType<Camera>().Size;

                Renderer.TargetCameraOveridePosition = VortexEngine.Master.LoadedScene.FindObjectOfType<Camera>().transform.Position;
            }
        
        Renderer.backend.ClearBackground(BackgroundColor);
        Performance.PopTask();

        Renderer.AddPositionOffset((Size / 2));

        Renderer.AddPositionOffset((Vector2.Zero - position));


        Performance.PushTask("Body Drawing");
        if (Debug.DrawBodys && VortexEngine.Master.LoadedScene != null){
            foreach (Body body in VortexEngine.Master.LoadedScene.Bodys)
            {
                try {
                    body.Draw();

                } catch {

                }
                
            }
        }
        Performance.PopTask();



        Performance.PushTask("Gizmo Rendering");
        if (VortexEngine.Master.LoadedScene != null){
            foreach (Body body in VortexEngine.Master.LoadedScene.Bodys)
            {
                body.DrawGizmos();
            }
        }
        

        Performance.PushTask("Editor Gizmo Drawing");
        if (VortexEngineEditor.Editor.SelectedBody != null){
            Body body = VortexEngineEditor.Editor.SelectedBody;
            Vector4 color = ImGui.GetStyle().Colors[(int)ImGuiCol.Button];


            if (body.transform.GetVisualScale().x != 0 || body.transform.GetVisualScale().y != 0)
            {
                Renderer.DrawRectangle(body.transform.Position, body.transform.GetVisualScale(), body.transform.Rotation, new Color(255,140,0, 50));
                Renderer.DrawEdgeRectangle(body.transform.Position, body.transform.GetVisualScale(), body.transform.Rotation, new Color(255,140,0, 255));
            }
            else
            {
                Renderer.DrawCircle(body.transform.Position, new Vector2(1, 1), new Color(255,140,0, 50));
                Renderer.DrawEdgeCircle(body.transform.Position, 1, 0, new Color(255,140,0, 255));
            }

            Vector2 tip = body.transform.Position + new Vector2(0, 50);
            Renderer.DrawArrow(body.transform.Position, tip, YArrowHovered ? new Color(0.0f, 255f, 0.0f, 255f) : new Color(0, 255f, 0, 255f/2f), body.transform.Rotation);
            Renderer.DrawArrow(body.transform.Position, body.transform.Position + new Vector2(50, 50), XArrowHovered ? new Color(255f, 0.0f, 0.0f, 255f) : new Color(255f, 0, 0, 255f/2f), body.transform.Rotation - 90);
            
            Rect YRect = new Rect(Vector2.RotateAboutOrigin(body.transform.Position + new Vector2(0, 30), body.transform.Position, body.transform.Rotation), new Vector2(10, 40), body.transform.Rotation);
            Rect XRect = new Rect(Vector2.RotateAboutOrigin(body.transform.Position + new Vector2(30, 0), body.transform.Position, body.transform.Rotation), new Vector2(10, 40), body.transform.Rotation - 90);

            XArrowHovered = XRect.Contains(SceneLocalMousePos);
            YArrowHovered = YRect.Contains(SceneLocalMousePos);
            
        }
        Performance.PopTask();
        Performance.PopTask();
        
        Performance.PushTask("Cleanup");
        Renderer.EndFrame();

        Renderer.ShowUI = true;

        Renderer.backend.EndTextureMode();

        VortexEngine.UNIT_SCALE = oldScale;
        Renderer.TargetCameraOveridePosition = new Vector2I(0, 0);
        Performance.PopTask();

    }

    public bool isDraggingScene;
    public bool isDraggingBody;

    public Vector2 SceneLastDragPos;
    public Vector2 BodyLastDragPos;

    public float XDragStartOffset;
    public float YDragStartOffset;

    public Vector2 SceneLocalMousePos;

    public Vector2 StartDown;

    public Vector2 EndDown;

    public override string GetTitle()
    {
        return "Scene";
    }

    public override void DrawContent(VortexEngineEditor editor)
    {

        SceneLocalMousePos = ImGui.GetMousePos() - ImGui.GetCursorScreenPos();
        SceneLocalMousePos -= (Vector2)ImGui.GetContentRegionAvail() / 2;
        SceneLocalMousePos += position * new Vector2(1, -1);
        SceneLocalMousePos /= Zoom;
        SceneLocalMousePos *= new Vector2(1, -1);
        
        if (Raylib.IsMouseButtonDown(MouseButton.Left))
        {
            StartDown = SceneLocalMousePos;
        }
        
        if (Raylib.IsMouseButtonReleased(MouseButton.Left))
        {
            EndDown = SceneLocalMousePos;
        }
        
        if (VortexEngine.Master.LoadedScene != null && !isDraggingBody){
            VortexEngineEditor.Editor.HoveredBody = null;
            foreach (var body in VortexEngine.Master.LoadedScene.Bodys)
            {
                Rect rect = body.transform.GetVisualRect();
                rect.Position += body.transform.GetVisualScale()/2;
                if (rect.Contains(SceneLocalMousePos)){
                    VortexEngineEditor.Editor.HoveredBody = body;
                }
            }
        }

        if (ImGui.IsWindowHovered())
            Zoom += Raylib.GetMouseWheelMove() * ZoomSpeed * Zoom;
        Zoom = Math.Clamp(Zoom, 0.1f, 100f);

        if (VortexEngineEditor.Editor.HoveredBody == null && !XDraging && !YDraging){
            #region SceneDragging
            Vector2 localMousePos = ImGui.GetCursorScreenPos();
            
            localMousePos += (Vector2)ImGui.GetContentRegionAvail();
            localMousePos -= (Vector2)(ImGui.GetContentRegionAvail() / 2);
            localMousePos *= new Vector2(-1, 1);
            localMousePos *= Zoom;


            if (Raylib.IsMouseButtonPressed(MouseButton.Middle) && ImGui.IsWindowHovered()){
                isDraggingScene = true;
                SceneLastDragPos = (localMousePos - (position * Zoom));
                Raylib.SetMouseCursor(MouseCursor.ResizeAll);
            }

            if (Raylib.IsMouseButtonDown(MouseButton.Middle) && isDraggingScene){
                position = (localMousePos - SceneLastDragPos) / Zoom;
            }

            if (Raylib.IsMouseButtonReleased(MouseButton.Middle)){
                isDraggingScene = false;
                Raylib.SetMouseCursor(MouseCursor.Default);
            }
            #endregion
        } else if (VortexEngineEditor.Editor.SelectedBody != null){
                if (VortexEngineEditor.Editor.HoveredBody == VortexEngineEditor.Editor.SelectedBody) {

                #region BodyDragging
                if (Raylib.IsMouseButtonPressed(MouseButton.Left) && ImGui.IsWindowHovered()){
                    isDraggingBody = true;
                    BodyLastDragPos = SceneLocalMousePos - VortexEngineEditor.Editor.SelectedBody.transform.Position;
                    Raylib.SetMouseCursor(MouseCursor.ResizeAll);
                }

                if (Raylib.IsMouseButtonDown(MouseButton.Left) && isDraggingBody){
                    VortexEngineEditor.Editor.SelectedBody.transform.Position = SceneLocalMousePos - BodyLastDragPos;
                }

                if (Raylib.IsMouseButtonReleased(MouseButton.Left)){
                    isDraggingBody = false;
                    Raylib.SetMouseCursor(MouseCursor.Default);
                }
                #endregion
            }
        }

        if (VortexEngineEditor.Editor.SelectedBody != null){
            if (XArrowHovered || XDraging){
                #region XDragging
                if (Raylib.IsMouseButtonPressed(MouseButton.Left) && ImGui.IsWindowHovered()){
                    XDraging = true;
                    XDragStartOffset = (VortexEngineEditor.Editor.SelectedBody.transform.Position - SceneLocalMousePos).x;
                    BodyLastDragPos = SceneLocalMousePos - VortexEngineEditor.Editor.SelectedBody.transform.Position;
                    Raylib.SetMouseCursor(MouseCursor.ResizeEw);

                }

                if (Raylib.IsMouseButtonDown(MouseButton.Left) && XDraging){
                    VortexEngineEditor.Editor.SelectedBody.transform.Position = Vector2.RotateAboutOrigin(new Vector2(SceneLocalMousePos.x - XLastDragPos.x + XDragStartOffset, VortexEngineEditor.Editor.SelectedBody.transform.Position.y), VortexEngineEditor.Editor.SelectedBody.transform.Position, VortexEngineEditor.Editor.SelectedBody.transform.Rotation);
                }

                if (Raylib.IsMouseButtonReleased(MouseButton.Left)){
                    XDraging = false;
                    Raylib.SetMouseCursor(MouseCursor.Default);
                }

                #endregion
            } else if (YArrowHovered || YDraging){
                #region YDragging
                if (Raylib.IsMouseButtonPressed(MouseButton.Left) && ImGui.IsWindowHovered()){
                    YDraging = true;
                    YDragStartOffset = (VortexEngineEditor.Editor.SelectedBody.transform.Position - SceneLocalMousePos).y;
                    BodyLastDragPos = SceneLocalMousePos - VortexEngineEditor.Editor.SelectedBody.transform.Position;
                    Raylib.SetMouseCursor(MouseCursor.ResizeNs);

                }

                if (Raylib.IsMouseButtonDown(MouseButton.Left) && YDraging){
                    VortexEngineEditor.Editor.SelectedBody.transform.Position = Vector2.RotateAboutOrigin(new Vector2(VortexEngineEditor.Editor.SelectedBody.transform.Position.x, SceneLocalMousePos.y - YLastDragPos.y + YDragStartOffset), VortexEngineEditor.Editor.SelectedBody.transform.Position, VortexEngineEditor.Editor.SelectedBody.transform.Rotation);

                }

                if (Raylib.IsMouseButtonReleased(MouseButton.Left)){
                    YDraging = false;
                    Raylib.SetMouseCursor(MouseCursor.Default);
                }

                #endregion
            }
        }
        if (VortexEngineEditor.Editor.HoveredBody != null){
            if (Raylib.IsMouseButtonReleased(MouseButton.Left) && StartDown.Distance(EndDown) < 0.125f && ImGui.IsWindowHovered() && !isDraggingScene && !XDraging && !YDraging && !XArrowHovered && !YArrowHovered){
                VortexEngineEditor.Editor.SelectedBody = VortexEngineEditor.Editor.HoveredBody;
            }
        }

        Size = ImGui.GetContentRegionAvail();
        if (true){
            Performance.PushTask("Scene Window Rendering");
            Render();
            Performance.PopTask();
        }
        
        if (renderTexture.id != -1)
        {
            if (Renderer.backend is RaylibRenderer renderer)
            {
                rlImGui.ImageSize(renderer.GetRenderTexture(renderTexture).Texture, Size);
            }
        }

    }
}
