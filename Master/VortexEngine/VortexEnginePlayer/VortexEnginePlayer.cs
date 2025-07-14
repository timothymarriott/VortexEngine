using System.Diagnostics;
using ImGuiNET;
using Raylib_cs;
using rlImGui_cs;
using System.Numerics;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;
using VortexEngine.Editor;
using VortexEngine.Internal.AssetManagement;
using VortexEngine.Rendering;
using VortexEngine.Rendering.Backends.Math;

namespace VortexEngine.Sample;

public class VortexEnginePlayer : VortexEngine
{
    public bool FontLoaded;
    public Theme MainTheme = new(new Vector4(0.544f, 0.000f, 0.000f, 1.000f));

    static void Main(string[] args)
    {
        new VortexEnginePlayer().Run();
    }

    public string tempDir =>
        Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), $"VortexEngine/Projects/ThirdParty/{Convert.ToBase64String(AssetManager.Sha256(assetDatabaseText).GetUTF8Bytes())}/");

    private string assetDatabaseText;
    
    public void Run()
    {
        
        StringWriter writer = new StringWriter();
        Log.stdout = Console.Out;
        Log.logOutput = writer;
        Console.SetOut(writer);
        
        
        string executableDirectory = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
        Log.Info(executableDirectory);
        ProjectPath = executableDirectory + "/";
        ProjectDataPath = ProjectPath + "Assets/";

        EditorDataPath = executableDirectory + "/";
        
        Log.Info(ProjectPath + "assets.pkk");
        
        if (File.Exists(ProjectPath + "assets.pkk"))
        {
            assetDatabaseText = File.ReadAllText(ProjectPath + "assets.pkk");
        }
        else
        {
            assetDatabaseText = Encoding.UTF8.GetString(ExeUnpack.ExtractPackedFile());
        }
        
        
        AssetManager.LoadFromFile(assetDatabaseText);
        byte[] fontData = AssetManager.ReadAllBytes("font", true);
        if (!Directory.Exists(tempDir))
        {
            Directory.CreateDirectory(tempDir);
        }
        File.WriteAllBytes(tempDir + "font.ttf", fontData);
        
        new ProjectContext().ReloadSampleProject(AssetManager.ReadAllBytes("assembly", true));

        Raylib.SetTraceLogLevel(TraceLogLevel.None);
        Raylib.InitWindow(100, 100, "Vortex Engine Test");

        Raylib.SetTargetFPS(60);

        Raylib.SetTraceLogLevel(TraceLogLevel.Error);

        rlImGui.Setup(enableDocking: true);
        
        ImGui.LoadIniSettingsFromDisk(tempDir + "imgui.ini");

        ImGuiIOPtr io = ImGui.GetIO();

        io.IniSavingRate = float.MaxValue;
        io.WantSaveIniSettings = false;


        ImGuiStylePtr style = ImGui.GetStyle();

        Style.ApplyTheme(Style.GetDefaultTheme());

        ImFontPtr font = ImGui.GetFont();


        style.FrameRounding = 4;

        Init();

        LoadScene(ProjectSettings.DefaultScene);
        
        


        Vector2I size = new Vector2I(100, 100);

        float tm = 0;
        
        if (File.Exists(ProjectPath + "imgui.ini"))
        {
            File.Delete(ProjectPath + "imgui.ini");
        }

        while (!Raylib.WindowShouldClose())
        {
            Raylib.BeginDrawing();

            Raylib.ClearBackground(Color.Black.ToRaylib());


            tm += Time.DeltaTime;

            if (tm > 2f)
            {
                tm = 0;
                ImGui.SaveIniSettingsToDisk(tempDir + "imgui.ini");
                if (File.Exists(ProjectPath + "imgui.ini"))
                {
                    File.Delete(ProjectPath + "imgui.ini");
                }
            }

            Vector2I windowsize = LoadedScene.FindObjectOfType<Camera>().Size;

            int display = Raylib.GetCurrentMonitor();

            size = LoadedScene.FindObjectOfType<Camera>().Size;

            FrameData frame = DrawFrame();
            if (frame.PixelData.id != 0){
                
                Renderer.backend.DrawRenderTexturePro(frame.PixelData, new Rect(0, 0, frame.PixelData.Width, frame.PixelData.Height), new Rect(0, 0, (int)(size.x * 5), (int)(size.y * 5)), Vector2.Zero, 0, Color.White);
            }

            rlImGui.Begin(Time.DeltaTime);


            if (!FontLoaded){
                FontLoaded = true;

                unsafe {
                    
                    string fontPath = tempDir + "font.ttf";
                    if (!File.Exists(fontPath))
                    {
                        Console.WriteLine("Font file not found at: " + fontPath);
                        throw new Exception("Font file not found at: " + fontPath);
                    }

                    
                    ImFontConfigPtr configuration = ImGuiNative.ImFontConfig_ImFontConfig();

                    configuration.OversampleH = 2;
                    configuration.OversampleV = 2;

                    configuration.MergeMode = false;

                    font = io.Fonts.AddFontFromFileTTF(fontPath, 16, configuration);

                    font.Scale = 1f;

                    io.Fonts.Build();

                    rlImGui.ReloadFonts();

                    ImGui.PushFont(font);
                    
                }

            } else {
                ImGui.PushFont(font);
            }

            Style.ApplyTheme(MainTheme);

            #region Dockspace
            ImGui.PushStyleColor(ImGuiCol.DockingEmptyBg, new System.Numerics.Vector4(0, 0, 0, 0));
            ImGui.PushStyleColor(ImGuiCol.WindowBg, new System.Numerics.Vector4(0, 0, 0, 0));

            ImGui.DockSpaceOverViewport();

            ImGui.PopStyleColor();
            #endregion

            if (VortexEngine.Master.LoadedScene != null)
                foreach (Body body in VortexEngine.Master.LoadedScene.Bodys)
                {

                    body.DrawDebugGui();
                }


            ImGui.PopFont();


            rlImGui.End();



            Raylib.SetWindowSize((int)(size.x * 5), (int)(size.y * 5));


            Raylib.EndDrawing();
        }
        
        ImGui.SaveIniSettingsToDisk(tempDir + "imgui.ini");
        

        rlImGui.Shutdown();
        
        if (File.Exists(ProjectPath + "imgui.ini"))
        {
            File.Delete(ProjectPath + "imgui.ini");
        }
    }

    public override bool PollInputDown(KeyCode key)
    {
        return Raylib.IsKeyDown((KeyboardKey)key);
    }

    public override bool PollInputPressed(KeyCode key)
    {
        return Raylib.IsKeyPressed((KeyboardKey)key);
    }

    public override void Start()
    {

    }

    public void Update()
    {

    }

}


public class ProjectContext : AssemblyLoadContext
{
    protected override Assembly Load(AssemblyName assemblyName)
    {
        return null;
    }

    public void ReloadSampleProject(byte[] assemblyData)
    {
        
        using (MemoryStream strm = new MemoryStream(assemblyData))
        {
            var newAssembly = this.LoadFromStream(strm);
        
            VortexEngine.ProjectAssembly = newAssembly;
        }
        
    }

    public ProjectContext() : base(isCollectible: true) { }
}

public static class ExeUnpack
{
    private const string MagicFooter = "ASSETDATABASE";

    public static byte[] ExtractPackedFile()
    {
        string exePath = Process.GetCurrentProcess().MainModule.FileName;

        using var stream = new MemoryStream(File.ReadAllBytes(exePath));
        using var reader = new BinaryReader(stream);

        if (stream.Length < MagicFooter.Length + 8)
            throw new InvalidDataException("Executable too small to contain embedded file.");


        reader.BaseStream.Seek(-MagicFooter.Length, SeekOrigin.End);
        Log.Info(stream.Length.ToString());
        string footer = Convert.ToBase64String(reader.ReadBytes(MagicFooter.Length));
        Log.Info(footer);
        if (footer != "QVNTRVREQVRBQkFTRQ==")
            throw new InvalidDataException("No packed file found or signature mismatch.");

        stream.Seek(-MagicFooter.Length - sizeof(int), SeekOrigin.End);
        int fileNameLength = reader.ReadInt32();

        long metaOffset = MagicFooter.Length + sizeof(int) + fileNameLength + sizeof(int);
        stream.Seek(-metaOffset, SeekOrigin.End);
        string fileName = Encoding.UTF8.GetString(reader.ReadBytes(fileNameLength));
        int fileSize = reader.ReadInt32();

        long fileDataOffset = stream.Length - metaOffset - fileSize;
        stream.Seek(fileDataOffset, SeekOrigin.Begin);
        return reader.ReadBytes(fileSize);
    }
}