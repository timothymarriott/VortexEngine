using CommandLine;
using VortexEngine.Internal.AssetManagement;

namespace VortexEngine.CLI.Pack; 

public static class PackCommand
{
    [Verb("pack", HelpText = "Pack files into an asset database.")]
    public class Options
    {
        [Option('i', "input", Required = true, HelpText = "Input directory.")]
        public string Input { get; set; }

        [Option('o', "output", Required = true, HelpText = "Output archive file.")]
        public string Output { get; set; }
        
        [Option('r', "root", Required = false, HelpText = "Root dir.")]
        public string Root { get; set; }
    }
    public static int Run(Options opts)
    {
        VortexEngine.InEditor = true;
        if (File.Exists(opts.Output))
        {
            
            AssetManager.LoadFromFile(File.ReadAllText(opts.Output));   
        }
        AssetDatabaseFile.UseCustomRoot = true;
        AssetDatabaseFile.CustomRoot = string.IsNullOrEmpty(opts.Root) ? opts.Input : opts.Root;
        AssetManager.AddData(opts.Input, false);
        File.WriteAllText(opts.Output, AssetManager.SerializeDatabase());
        return 0;
    }
}