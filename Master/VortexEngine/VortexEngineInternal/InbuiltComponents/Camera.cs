using VortexEngine.Rendering;

namespace VortexEngine;

public class Camera : Component
{
    public Vector2 Size = new Vector2I(800, 600);

    public Color ClearColor = Color.White;

    RenderTexture renderTexture = RenderTexture.Null;

    public override void Start()
    {

    }

    public FrameData Render()
    {
        
        
        Performance.PushTask("Renderer Initialzation");
        
            
        FrameData result = new FrameData();
        if (transform == null) return result;
        int width = (int)(Size.x * VortexEngine.UNIT_SCALE);
        int height = (int)(Size.y * VortexEngine.UNIT_SCALE);
        if (renderTexture.Width != width || renderTexture.Height != height)
        {
            Performance.PushTask("Resize Renderer");
            {
                Renderer.backend.UnloadRenderTexture(renderTexture);
        
                renderTexture = Renderer.backend.LoadRenderTexture(new Vector2((int)(Size.x * VortexEngine.UNIT_SCALE), (int)(Size.y * VortexEngine.UNIT_SCALE)));
                Performance.PopTask();
            }
        }

        Renderer.backend.BeginTextureMode(renderTexture);

        Performance.PushTask("Begin Frame");
        Renderer.BeginFrame(renderTexture);
        Performance.PopTask();

        Renderer.backend.ClearBackground(ClearColor);

        Renderer.AddPositionOffset((Size / 2) * VortexEngine.UNIT_SCALE);

        Renderer.AddPositionOffset((Vector2.Zero - transform.Position) * VortexEngine.UNIT_SCALE);

        Performance.PopTask();
    



        Performance.PushTask("Main Rendering");
        {

            IOrderedEnumerable<Body> sorted = VortexEngine.Master.LoadedScene.Bodys.OrderBy(b => b.SortingOrder);
            
            Performance.PushTask("Body Drawing");
            foreach (Body body in sorted)
            {
                Performance.PushTask($"Draw {body.Name}");
                body.Draw();
                Performance.PopTask();

            }
            Performance.PopTask();
            
            Performance.PushTask("UI Drawing");
            Renderer.BeginUIMode();
            foreach (Body body in sorted)
            {
                Performance.PushTask($"Draw {body.Name} UI");
                body.DrawUI();
                Performance.PopTask();

            }
            Renderer.EndUIMode();
            Performance.PopTask();


            Performance.PushTask("Gizmo Drawing");
            if (VortexEngine.Master.DrawGizmos)
                foreach (Body body in VortexEngine.Master.LoadedScene.Bodys)
                {
                    body.DrawGizmos();

                }
            Performance.PopTask();

            Performance.PopTask();
            
        }


        Performance.PushTask("Renderer Cleanup");
        {
            Performance.PushTask("End Frame");
            Renderer.EndFrame();
            Performance.PopTask();

            Performance.PushTask("End Texture Mode");
            Renderer.backend.EndTextureMode();
            Performance.PopTask();
            
            {
                Performance.PushTask("Loading Image from render texture");
                result.PixelData = renderTexture;

                result.Size = Size;

                Performance.PopTask();
            }
            
            Performance.PopTask();
        }
        
        

        return result;

    }

    public override void DrawGizmos()
    {
        Renderer.DrawEdgeRectangle(transform.Position, Size, 0, Color.White);
    }
}
