

using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.Json;
using ImGuiNET;
using Raylib_cs;
using rlImGui_cs;
using VortexEngine.Rendering.Backends.Math;
using Color = Raylib_cs.Color;

namespace VortexEngine.Editor.Hub;

public class VortexHubMeta {
    public List<string> Projects = new List<string>();
}

public static class VortexHub {
    public static string DATA_PATH = Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "VortexEngine", "hub.json");

    public static string EDITOR_PATH = Path.Join("..", "VortexEngineStandalone", "bin", "Debug", "net8.0", "VortexEngineEditor.exe");

    public static string GetProject() {
        if (!Directory.Exists(Path.GetDirectoryName(DATA_PATH))){
            Directory.CreateDirectory(Path.GetDirectoryName(DATA_PATH));
        }
        
        Raylib.InitWindow((int)(800), (int)(600), "Vortex Hub");

        Raylib.SetTargetFPS(60);

        Raylib.SetTraceLogLevel(TraceLogLevel.Error);

        rlImGui.Setup(enableDocking: true);

        ImGui.PushStyleVar(ImGuiStyleVar.FrameRounding, 2);
        
        List<string> Projects = GetMeta().Projects;

        while (!Raylib.WindowShouldClose()){
            Raylib.BeginDrawing();

            Raylib.ClearBackground(Color.Black.ToRaylib());

            rlImGui.Begin(Raylib.GetFrameTime());
            
            ImGui.PushStyleColor(ImGuiCol.DockingEmptyBg, new System.Numerics.Vector4(0, 0, 0, 0));
            ImGui.PushStyleColor(ImGuiCol.WindowBg, new System.Numerics.Vector4(0, 0, 0, 0));

            ImGui.DockSpaceOverViewport();

            ImGui.PopStyleColor();

            ImGui.Begin("Projects", ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoDocking | ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoDecoration | ImGuiWindowFlags.NoResize);

            if (ImGui.Button("Add")){
                #if MAC_OS
                string proj = NSOpenPanel.ShowOpenPanel() + "/";

                if (IsValidProject(proj)){
                    Projects.Add(proj);
                    SaveMeta(new VortexHubMeta{Projects = Projects});
                }
                #else
                var selectDirectoryDialog = NativeFileDialogSharp.Dialog.FolderPicker();

                if (selectDirectoryDialog.IsOk)
                {
                    if (IsValidProject(selectDirectoryDialog.Path)){
                        Projects.Add(selectDirectoryDialog.Path);
                        SaveMeta(new VortexHubMeta{Projects = Projects});
                    }
                }
                #endif
            }

            foreach (string Project in Projects){
                if (IsValidProject(Project)){
                    if (ImGui.Button(GetProjectName(Project))){
                        Log.Info(Process.GetCurrentProcess().MainModule.FileName);
                        ProcessStartInfo info = new ProcessStartInfo();
                        info.FileName = Process.GetCurrentProcess().MainModule.FileName;
                        info.Arguments = Project + "/";
                        info.WorkingDirectory = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
                        
                        Process process = Process.Start(info);
                        Environment.Exit(0);
                    }
                } else {
                    ImGui.TextColored(new System.Numerics.Vector4(255, 0, 0, 255), Project);
                }
            }

            ImGui.End();

            rlImGui.End();

            Raylib.EndDrawing();

        }

        rlImGui.Shutdown();

        Raylib.CloseWindow();

        return "NULL";
    }

    public static VortexHubMeta GetMeta(){
        if (!Directory.Exists(Path.GetDirectoryName(DATA_PATH))){
            Directory.CreateDirectory(Path.GetDirectoryName(DATA_PATH));
        }
        if (!File.Exists(DATA_PATH)){
            File.WriteAllText(DATA_PATH, JsonSerializer.Serialize(new VortexHubMeta(), new JsonSerializerOptions {WriteIndented = true, IncludeFields = true}));
        }
        VortexHubMeta? meta = JsonSerializer.Deserialize<VortexHubMeta>(File.ReadAllText(DATA_PATH), new JsonSerializerOptions {WriteIndented = true, IncludeFields = true});
        if (meta != null){
            return meta;
        }
        return new VortexHubMeta();
    }

    public static void SaveMeta(VortexHubMeta meta){
        if (!Directory.Exists(Path.GetDirectoryName(DATA_PATH))){
            Directory.CreateDirectory(Path.GetDirectoryName(DATA_PATH));
        }
        File.WriteAllText(DATA_PATH, JsonSerializer.Serialize(meta, new JsonSerializerOptions {WriteIndented = true, IncludeFields = true}));
    }

    public static bool IsValidProject(string project){
        if (!Directory.Exists(Path.GetDirectoryName(DATA_PATH))){
            Directory.CreateDirectory(Path.GetDirectoryName(DATA_PATH));
        }
        if (!Directory.Exists(project)){
            return false;
        }
        if (Directory.Exists(project + "/Assets/")){
            return true;
        }
        return false;
    }

    public static string GetProjectName(string project){
        if (!Directory.Exists(Path.GetDirectoryName(DATA_PATH))){
            Directory.CreateDirectory(Path.GetDirectoryName(DATA_PATH));
        }
        if (!IsValidProject(project)){
            return "Invalid";
        } else {
            return new DirectoryInfo(project).Name;
        }
    }
}
