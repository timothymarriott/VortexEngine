using System.Diagnostics;
using System.Numerics;
using System.Reflection;
using ImGuiNET;
using Raylib_cs;
using rlImGui_cs;
using VortexEngine.Internal.AssetManagement;
using VortexEngine.Rendering;
using VortexEngine.Editor.Windows;
using VortexEngine.Rendering.Backends;
using VortexEngine.Rendering.Backends.Math;

namespace VortexEngine.Editor;

public class VortexEngineEditor : VortexEngine
{
    static void Main(string[] args)
    {
        new VortexEngineEditor().Run();
    }

    public Dictionary<Type, EditorWindow> windows = new Dictionary<Type, EditorWindow>();
    
    public Body? SelectedBody;
    public Body? HoveredBody;

    public bool GameRunning = false;

    public static VortexEngineEditor Editor;

    public FrameData lastFrame = new() {PixelData = RenderTexture.Null};

    
    public Vector4 ThemeColor = new(0.192f, 0.000f, 0.380f, 1.000f);
    private int selectedBuildTarget;

    public bool showBuildMenu;
    public bool showBuildMenuQueued;

    public static bool SceneReloadQueued = false;

    public T GetWindow<T>() where T : EditorWindow, new()
    {
        if (!windows.ContainsKey(typeof(T)))
        {
            T tmp = new T();
            windows.Add(typeof(T), tmp);
            return tmp;
        }

        T? res = windows[typeof(T)] as T;
        return res;
    }

    public override float GetTime()
    {
        return (float)Raylib.GetTime();
    }

    public override float GetFrameTime()
    {
        return Raylib.GetFrameTime();
    }

    public void Run()
    {

        Renderer.backend = new RaylibRenderer();
        Renderer.guiBackend = new RLImguiRenderer();
        
        Raylib.SetTraceLogLevel(TraceLogLevel.Error);

        TextWriter stdout = Console.Out;
        
        StringWriter debuglog = new StringWriter();
        Console.SetOut(debuglog);

        Log.stdout = stdout;
        Log.logOutput = debuglog;
        
        InEditor = true;

        #if DEBUG
            ProjectPath = new DirectoryInfo("../../../../../../Project/BallGameNameLame/").FullName;
        #else
        
            if (Environment.GetCommandLineArgs().Length != 2){
                ProjectPath = "NULL";
            } else {
                ProjectPath = Environment.GetCommandLineArgs()[1];
            }
        #endif
        
        if (!Directory.Exists(ProjectPath) || !Directory.Exists(ProjectDataPath)){
            #if DEBUG
            var res = NativeFileDialogSharp.Dialog.FolderPicker();
            if (res.IsOk && File.Exists(Path.Join(res.Path, "Assets/scene.vscn")))
            {
                ProjectPath = res.Path + Path.DirectorySeparatorChar;
            }
            else
            {
                throw new Exception("Invalid debug project: " + new DirectoryInfo(ProjectPath).FullName);
            }
            
            
            #else

            string proj = VortexHub.GetProject();
            if (proj == "NULL"){
                return;
            }
            ProjectPath = proj;
            #endif
        }
        
        EditorDataPath = Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "VortexEngine") + Path.DirectorySeparatorChar;

        if (!Directory.Exists(EditorDataPath))
        {
            Log.Info("No editor data selected, please select the editor data folder.");
            var res = NativeFileDialogSharp.Dialog.FolderPicker();
            if (res.IsOk && File.Exists(Path.Join(res.Path, "font.ttf")))
            {
                Directory.CreateDirectory(EditorDataPath);
                File.WriteAllText(Path.Join(EditorDataPath, "ref.txt"), res.Path + Path.DirectorySeparatorChar);
                EditorDataPath = res.Path + Path.DirectorySeparatorChar;
            }
            else
            {
                throw new Exception("Invalid editor data folder: " + new DirectoryInfo(Path.Join(EditorDataPath)).FullName);
            }
        } else if (File.Exists(Path.Join(EditorDataPath, "ref.txt")))
        {
            EditorDataPath = File.ReadAllText(Path.Join(EditorDataPath, "ref.txt"));
        }
        
        

        HotReload.ClearBuilds();
        
        Editor = this;
        
        Raylib.SetConfigFlags(ConfigFlags.ResizableWindow | ConfigFlags.HighDpiWindow);

        int targetMonitor = 0;

        Raylib.InitWindow((int)(Raylib.GetMonitorWidth(targetMonitor) / 3.0f), (int)(Raylib.GetMonitorHeight(targetMonitor) / 3.0f), "Vortex Engine Test");

        Raylib.SetExitKey(KeyboardKey.Null);
        
        Raylib.SetWindowMonitor(targetMonitor);

        int windowWidth = (int)(Raylib.GetMonitorWidth(targetMonitor) * 0.95f);
        int windowHeight = (int)(Raylib.GetMonitorHeight(targetMonitor) * 0.85f);

        Raylib.SetWindowSize(windowWidth, windowHeight);

        Raylib.SetWindowPosition((int)(Raylib.GetMonitorPosition(targetMonitor).X + Raylib.GetMonitorWidth(targetMonitor) / 2 - windowWidth / 2), (int)(Raylib.GetMonitorPosition(targetMonitor).Y + Raylib.GetMonitorHeight(targetMonitor) / 2 - windowHeight / 2));

        Icons.LoadIcons();

        rlImGui.Setup(enableDocking: true);
        
        
        
        ImGui.GetIO().ConfigErrorRecoveryEnableAssert = false;
        
        ImGui.LoadIniSettingsFromDisk(Path.Join(EditorDataPath, "layouts", "default.ini"));
        Log.Info(Path.Join(EditorDataPath, "layouts", "default.ini"));
        
        Init();

        HotReload.Reload();
        HotReload.SetupWatcher();

        Start();

        if (LoadedScene != null){
            foreach (Body body in LoadedScene.InstantiateCache)
            {
                LoadedScene.Bodys.Add(body);

            }
            LoadedScene.InstantiateCache.Clear();

            LoadedScene.Start();
        }
        
        ImGuiStylePtr style = ImGui.GetStyle();

        Style.ApplyTheme(Style.GetDefaultTheme());

        float colorShift = 0;

        style.FrameRounding = 4;

        GetWindow<EditorSettingsWindow>();
        GetWindow<SceneSettingsWindow>();
        GetWindow<FileBrowserWindow>();
        GetWindow<GameWindow>();
        GetWindow<HierarchyWindow>();
        GetWindow<InspectorWindow>();
        GetWindow<LogWindow>();
        
        GetWindow<TimerWindow>();
        GetWindow<UIWindow>();
        GetWindow<PerformanceWindow>();
        
        GetWindow<SceneWindow>();

        while (!Raylib.WindowShouldClose())
        {
            Performance.PushTask("Editor Frame");

            if (SceneReloadQueued)
            {
                SceneReloadQueued = false;
                Performance.PushTask("Scene Reload");
                if (ProjectAssembly != null)
                {
                    LoadScene("scene.vscn");
                }

                HotReload.StatusMessage = "Complete!";
                Performance.PopTask();
            }
            
            Performance.PushTask("Frame Setup");
            HotReload.IsBuilding = !HotReload.BuildCompleted;
            
            Style.Begin();

            Raylib.BeginDrawing();

            Raylib.ClearBackground(Color.Black.ToRaylib());
            if (GetWindow<GameWindow>().FrameTexture.id != -1){
                GetWindow<GameWindow>().FrameTexture = RenderTexture.Null;
            }
            
            Performance.PopTask();

            Performance.PushTask("Update Frame Texture");
            FrameData frame = lastFrame;

            if (LoadedScene != null)
                frame = DrawFrame(GameRunning, true);
            
            if (frame.PixelData.id != 0){
                GetWindow<GameWindow>().FrameTexture = frame.PixelData;
            } else {
                GetWindow<GameWindow>().FrameTexture = RenderTexture.Null;
            }
            
            lastFrame = frame;
            Performance.PopTask();
            
            Performance.PushTask("rlimgui Setup");
            rlImGui.Begin(Time.DeltaTime);
            Performance.PopTask();
            
            ImGui.PushFont(Style.font);
            
            #region Dockspace
            ImGui.PushStyleColor(ImGuiCol.DockingEmptyBg, new System.Numerics.Vector4(0, 0, 0, 0));
            ImGui.PushStyleColor(ImGuiCol.WindowBg, new System.Numerics.Vector4(0, 0, 0, 0));

            ImGui.DockSpaceOverViewport();

            ImGui.PopStyleColor(2);
            #endregion
            
            if (HotReload.IsBuilding)
                ImGui.BeginDisabled();
            Performance.PushTask("Menu Bar");
            if (ImGui.BeginMainMenuBar())
            {

                if (ImGui.BeginMenu("File"))
                {
                    if (ImGui.BeginMenu("Build") && !GameRunning)
                    {
                        
                        int index = 0;
                        foreach (var targ in Enum.GetNames<BuildTarget>())
                        {
                            bool v = selectedBuildTarget == index;
                            ImGui.Checkbox(targ, ref v);
                            if (v)
                            {
                                selectedBuildTarget = index;
                            }

                            index++;
                        }
                        
                        if (ImGui.MenuItem("Build"))
                        {
                            
                            showBuildMenuQueued = true;
                            
                        }
                        ImGui.EndMenu();
                    }
                    ImGui.EndMenu();
                }

                if (ImGui.BeginMenu("Game")){
                    if (ImGui.MenuItem("Toggle Playing", "Cmd+P")){
                        GameRunning = !GameRunning;
                    }
                    ImGui.EndMenu();
                }

                if (ImGui.BeginMenu("Pause", GameRunning)){
                    GameRunning = false;
                    ImGui.EndMenu();
                }

                if (ImGui.BeginMenu("Play", !GameRunning)){
                    GameRunning = true;
                    ImGui.EndMenu();
                }

                if (ImGui.BeginMenu("Stop", GameRunning)){
                    GameRunning = false;
                    LoadScene("scene.vscn");
                    ImGui.CloseCurrentPopup();
                    ImGui.EndMenu();
                }

                if (ImGui.BeginMenu("Save") && !GameRunning){

                    LoadedScene.Save();
                    ImGui.CloseCurrentPopup();
                    ImGui.EndMenu();
                }

                if (ImGui.BeginMenu("Load") && !GameRunning){

                    LoadScene("scene.vscn");
                    ImGui.CloseCurrentPopup();
                    ImGui.EndMenu();
                }

                if (ImGui.BeginMenu("Compile") && !GameRunning){

                    HotReload.Reload();
                    ImGui.CloseCurrentPopup();
                    ImGui.EndMenu();
                }

                if (ImGui.BeginMenu("FPS: " + (1.0f / Time.DeltaTime).ToString(), false)){
                    ImGui.CloseCurrentPopup();
                    ImGui.EndMenu();
                }
                
                ImGui.EndMainMenuBar();
            }
            
            Performance.PopTask();

            if (showBuildMenuQueued)
            {
                showBuildMenuQueued = false;
                showBuildMenu = true;
                ImGui.OpenPopup("Build Menu");
            }

            if (showBuildMenu)
            {
                if (ImGui.BeginPopupModal("Build Menu", ref showBuildMenu, ImGuiWindowFlags.AlwaysAutoResize))
                {

                    ImGui.Text("Select build target.");
                    ImGui.Separator();

                    ImGui.Combo("Target", ref selectedBuildTarget,
                        string.Join("\0", Enum.GetNames(typeof(BuildTarget))));

                    if (ImGui.Button("Cancel"))
                    {
                        showBuildMenu = false;
                        ImGui.CloseCurrentPopup();

                    }

                    ImGui.SameLine();
                    
                    if (ImGui.Button("Build"))
                    {

                        NativeFileDialogSharp.DialogResult outputdir = NativeFileDialogSharp.Dialog.FolderPicker();
                        if (outputdir.IsOk){
                            Console.WriteLine(outputdir.Path);
                            Build(outputdir.Path, Enum.Parse<BuildTarget>(Enum.GetNames(typeof(BuildTarget))[selectedBuildTarget]));
                        }

                        showBuildMenu = false;
                        ImGui.CloseCurrentPopup();
                    }

                    ImGui.EndPopup();
                }
            }

            if (Raylib.IsKeyDown(KeyboardKey.LeftSuper) && Raylib.IsKeyPressed(KeyboardKey.P)){
                GameRunning = !GameRunning;
            }
            
            Performance.PushTask("Draw Editor Windows");
            Performance.PushTask("Copy Window List");
            List<EditorWindow> _windows = windows.Values.ToList();
            Performance.PopTask();
            foreach (var window in _windows)
            {
                Performance.PushTask(window.GetType().Name);
                window.Draw();
                Performance.PopTask();
            }
            Performance.PopTask();

            if (LoadedScene != null)
                foreach (Body body in LoadedScene.Bodys)
                {

                    body.DrawDebugGui();
                }

            if (HotReload.IsBuilding)
                ImGui.EndDisabled();
            
            
            if (HotReload.ReloadQueued){
                HotReload.ReloadQueued = false;
                GameRunning = false;
                HotReload.Reload();
            }
            
            
            Performance.PushTask("Frame cleanup");
            Style.End();

            Performance.PushTask("Imgui Draw");
            rlImGui.End();
            Performance.PopTask();
            
            Performance.PushTask("Raylib Draw");
            Raylib.EndDrawing();
            Performance.PopTask();
            
            Performance.PopTask();
            Performance.PopTask();
            if (!GetWindow<PerformanceWindow>().Paused)
                Performance.Populate();
            
            Performance.CompletedTasks.Clear();
            Performance.ProcessingStack.Clear();
            
        }
        
        Style.Shutdown();
        
        HotReload.ClearBuilds(HotReload.buildCount);

        rlImGui.Shutdown();
        
        debuglog.Close();

    }
    
    public override bool PollInputDown(KeyCode key)
    {
        return Raylib.IsKeyDown((KeyboardKey)key);
    }

    public override bool PollInputPressed(KeyCode key)
    {
        return Raylib.IsKeyPressed((KeyboardKey)key);
    }

    public void Build(string path, BuildTarget target){
        string projectName = new DirectoryInfo(ProjectPath).Name;
        
        AssetManager.AddData(ProjectDataPath);
        AssetManager.AddFile(File.ReadAllBytes(EditorDataPath + "font.ttf"), "font");
        AssetManager.AddFile(File.ReadAllBytes(ProjectPath + "/build/net8.0/Project.dll"), "assembly");
        
        if (target == BuildTarget.Windows)
        {
            BuildWindows(projectName, path);
        } 

        if (target == BuildTarget.MacosArm64 || target == BuildTarget.MacosX64)
        {
            BuildMacos(projectName, path, target);
        }
        
        ProcessStartInfo process = new ProcessStartInfo();
        process.FileName = path;
        process.UseShellExecute = true;

        Process proc = Process.Start(process);
    }

    public void BuildMacos(string projectName, string path, BuildTarget target)
    {
        string appDir = Path.Join(path, $"{projectName}.app");
        string contentsDir = Path.Join(appDir, "Contents");
        string exeDir = Path.Join(appDir, "Contents", "MacOS");
        string resourcesDir = Path.Join(contentsDir, "Resources");
        Directory.CreateDirectory(exeDir);
#if DEBUG
        File.Copy(Path.Join("..", "..", "..", "..", "VortexEnginePlayer", "bin", "LinuxRelease", "net8.0", target == BuildTarget.MacosArm64 ? "osx-arm64" : "osx-x64", "publish", "VortexEnginePlayer"), Path.Join(exeDir, "VortexEnginePlayer"), true);
#else
        File.Copy(Path.Join(".", "Player", target == BuildTarget.MacosArm64 ? "osx-arm64-VortexEnginePlayer" : "osx-x64-VortexEnginePlayer"), Path.Join(exeDir, "VortexEnginePlayer"), true);
#endif

        if (Environment.OSVersion.Platform == PlatformID.MacOSX)
        {
            var startInfo = new ProcessStartInfo()
            {
                FileName = "chmod",
                Arguments = $"+x {Path.Join(exeDir, projectName)}"
            };
            var process = Process.Start(startInfo);
            process.Start();
            process.WaitForExit();
        }

        Directory.CreateDirectory(resourcesDir);
        File.WriteAllText(Path.Join(exeDir, "assets.pkk"), AssetManager.SerializeDatabase());
        File.WriteAllText(Path.Join(contentsDir, "info.plist"), $"""
                                                                <?xml version="1.0" encoding="UTF-8"?>
                                                                <!DOCTYPE plist PUBLIC "-//Apple//DTD PLIST 1.0//EN" "http://www.apple.com/DTDs/PropertyList-1.0.dtd">
                                                                <plist version="1.0">
                                                                <dict>
                                                                    <key>CFBundleDevelopmentRegion</key>
                                                                    <string>en</string>
                                                                    <key>CFBundleExecutable</key>
                                                                    <string>VortexEnginePlayer</string>
                                                                    <key>CFBundleIconFile</key>
                                                                    <string>AppIcon</string>
                                                                    <key>CFBundleIdentifier</key>
                                                                    <string>(PRODUCT_BUNDLE_IDENTIFIER)</string>
                                                                    <key>CFBundleInfoDictionaryVersion</key>
                                                                    <string>6.0</string>
                                                                    <key>CFBundleName</key>
                                                                    <string>{projectName}</string>
                                                                    <key>CFBundlePackageType</key>
                                                                    <string>APPL</string>
                                                                    <key>CFBundleShortVersionString</key>
                                                                    <string>1.0</string>
                                                                    <key>CFBundleVersion</key>
                                                                    <string>1</string>
                                                                    
                                                                </dict>
                                                                </plist>
                                                                """);
    }

    public void BuildWindows(string projectName, string path)
    {
        File.WriteAllText(Path.Join(path, "assets.pkk"), AssetManager.SerializeDatabase());
        
#if DEBUG
        File.Copy(Path.Join("..", "..", "..", "..", "VortexEnginePlayer", "bin", "WindowsRelease", "net8.0", "win-x64", "publish", "VortexEnginePlayer.exe"), Path.Join("player.exe"), true);
        
#else
        File.Copy(Path.Join(".", "Player", "win-x64-VortexEnginePlayer.exe"), path + $"/player.exe", true);
#endif
        
        ExePacker.PackFileIntoExe(Path.Join(path, "player.exe"), Path.Join(path, "/assets.pkk"), Path.Join(path,$"/{projectName}.exe"));
        File.Delete(Path.Join(path, "player.exe"));
        File.Delete(Path.Join(path, "assets.pkk"));

    }
    
    public override void Start()
    {

    }

}

public enum BuildTarget
{
    Windows,
    MacosArm64,
    MacosX64
}