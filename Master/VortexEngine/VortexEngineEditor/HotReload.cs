using System.Reflection;
using System.Runtime.Loader;
using Microsoft.Build.Execution;
using Microsoft.Build.Framework;
using Microsoft.Build.Locator;
using Microsoft.Build.Logging;

namespace VortexEngine.Editor;

public static class HotReload
{

    public static HotReloadContext context;

    public static int buildCount = 0;

    public static bool IsBuilding;

    public static FileSystemWatcher watcher;

    public static bool ReloadQueued;

    public static bool BuildCompleted = true;

    public static string StatusMessage = "Reloading...";

    public static void SetupWatcher(){

        watcher = new FileSystemWatcher(VortexEngine.ProjectDataPath, "*.cs");
        watcher.IncludeSubdirectories = true;
        watcher.NotifyFilter = NotifyFilters.LastWrite;

        watcher.Changed += OnChanged;
        watcher.EnableRaisingEvents = true;
        
    }

    private static void OnChanged(object sender, FileSystemEventArgs e)
    {
        ReloadQueued = true;

    }

    public static bool BuildProject(string projectPath)
    {
        string projPath = VortexEngine.ProjectPath;
        string configuration = "Debug";
        string targetFramework = "net8.0";

        var globalProps = new Dictionary<string, string>
        {
            { "Configuration", configuration },
            { "Platform", "AnyCPU" },
            { "OutputPath", projPath + "build\\" + targetFramework + "\\" }
        };
        
        var logger = new ConsoleLogger(LoggerVerbosity.Minimal);


        var parameters = new BuildParameters
        {
            Loggers = new List<ILogger> { logger, new HotReloadLogger() }
        };

        var restoreRequest = new BuildRequestData(projectPath, globalProps, null, new[] { "Restore" }, null);
        var restoreResult = BuildManager.DefaultBuildManager.Build(parameters, restoreRequest);
        if (restoreResult.OverallResult != BuildResultCode.Success)
        {
            Log.Error("NuGet Restore Failed.");
            return false;
        }
        
        var buildRequest = new BuildRequestData(projectPath, globalProps, null, new[] { "Build" }, null);

        var result = BuildManager.DefaultBuildManager.Build(parameters, buildRequest);
        
        
        bool success = result.OverallResult == BuildResultCode.Success;

        if (success)
        {
            buildCount++;
            string tempDll = projPath + $"build/{targetFramework}/Project-temp-{buildCount}.dll";
            string builtDll = projPath + $"build/{targetFramework}/Project.dll";

            if (File.Exists(tempDll))
                File.Delete(tempDll);

            if (File.Exists(builtDll))
                File.Copy(builtDll, tempDll);
        }

        return success;
        
    }

    public static void ClearBuilds(int count = -1){
        string projPath = VortexEngine.ProjectPath;

        for (int i = 0; i < (count == -1 ? 100 : HotReload.buildCount + 5); i++)
        {
            if (File.Exists(projPath + "build\\net8.0\\Project-temp-" + i.ToString() + ".dll")){
                try{
                    File.Delete(projPath + "build\\net8.0\\Project-temp-" + i.ToString() + ".dll");
                } catch{
                }
            }
        }
    }

    public static void Reload()
    {
        StatusMessage = "Reloading...";
        if (!MSBuildLocator.IsRegistered)
        {
            Log.Info("Registering MSBuild paths...");
            MSBuildLocator.RegisterDefaults();
        }
        Thread buildThread = new Thread(() => {
            IsBuilding = true;
            BuildCompleted = false;
            string projPath = VortexEngine.ProjectPath;

            StatusMessage = "Unloading...";
            VortexEngine.Master.LoadedScene = null;

            if (context != null) {
                context.Unload();
            }
            
            GC.Collect();
            GC.WaitForPendingFinalizers();
            
            context = new HotReloadContext();
            
            StatusMessage = "Building...";

            VortexEngine.ProjectAssembly = null;
            if (BuildProject(projPath + "Project.csproj")){
                Log.Info("Compiled succesfully.");
            } else {
                Log.Error("Compilation Failed");
            }

            StatusMessage = "Loading...";

            context.ReloadSampleProject(projPath + "build/net8.0/Project-temp-" + buildCount.ToString() + ".dll");

            StatusMessage = "Reloading scene...";

            VortexEngineEditor.SceneReloadQueued = true;
            
            
            BuildCompleted = true;
        });
        buildThread.Start();

    }
}

public class HotReloadContext : AssemblyLoadContext
{
    protected override Assembly Load(AssemblyName assemblyName)
    {
        return null;
    }

    public void ReloadSampleProject(string assemblyPath)
    {

        if (!File.Exists(assemblyPath))
        {
            Log.Error($"Trying to load a non existant assembly from \"{assemblyPath}\"");
        }
        
        try
        {
            var newAssembly = LoadFromAssemblyPath(assemblyPath);
            VortexEngine.ProjectAssembly = newAssembly;
            Log.Info("Project Assembly Loaded");
        }
        catch (Exception e)
        {
            if (e is FileNotFoundException)
            {
                Log.Error("Assembly Reload Failed: File not found.");
            }
            else
            {
                Log.Error($"Assembly Reload Failed: {e.GetType().Name}");
            }
        }
        
    }

    public HotReloadContext() : base(isCollectible: true) { }
}


public class HotReloadLogger : ILogger
{
    public string LatestMessage { get; private set; } = "";

    public LoggerVerbosity Verbosity { get; set; } = LoggerVerbosity.Minimal;
    public string Parameters { get; set; }

    public void Initialize(IEventSource eventSource)
    {
        if (eventSource == null) throw new ArgumentNullException(nameof(eventSource));

        
        eventSource.ProjectStarted += (sender, args) =>
        {
            LatestMessage = args.Message;
            Log.ShowStackTrace = false;
            Log.Info("Building project: " + args.ProjectFile);
            HotReload.StatusMessage = "Building project: " + args.ProjectFile;
            Log.ShowStackTrace = true;
        };
        
        eventSource.ProjectFinished += (sender, args) =>
        {
            LatestMessage = args.Message;
            Log.ShowStackTrace = false;
            if (args.Succeeded)
            {
                Log.Info($"Building \"{args.ProjectFile}\" succeeded.");
                HotReload.StatusMessage = $"Building \"{args.ProjectFile}\" succeeded.";
            }
            else
            {
                Log.Info($"Failed to build \"{args.ProjectFile}\".");
                HotReload.StatusMessage = $"Failed to build \"{args.ProjectFile}\".";
            }
            Log.ShowStackTrace = true;
        };

        
        eventSource.WarningRaised += (sender, args) =>
        {
            LatestMessage = $"Warning: {args.Message}";
            Log.ShowStackTrace = false;
            Log.Warning(args.Message);
            Log.ShowStackTrace = true;
        };

        eventSource.ErrorRaised += (sender, args) =>
        {
            LatestMessage = $"Error: {args.Message}";
            Log.ShowStackTrace = false;
            Log.Error(args.Message);
            Log.ShowStackTrace = true;
        };
    }

    public void Shutdown()
    {
        
    }
}