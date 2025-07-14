using CommandLine;
using VortexEngine.Internal.AssetManagement;

namespace VortexEngine.CLI.Extract;

public static class ExtractCommand
{
    [Verb("extract", HelpText = "Extract files into an asset database.")]
    public class Options
    {
        [Option('i', "input", Required = true, HelpText = "Input asset pkk file.")]
        public string Input { get; set; }

        [Option('o', "output", Required = true, HelpText = "Output dir.")]
        public string Output { get; set; }
    }
    public static int Run(Options opts)
    {
        VortexEngine.InEditor = true;
        AssetManager.LoadFromFile(File.ReadAllText(opts.Input));
        AssetManager.GetDatabase().Extract(opts.Output);
        return 0;
    }
}