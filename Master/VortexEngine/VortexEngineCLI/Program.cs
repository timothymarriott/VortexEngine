using CommandLine;
using VortexEngine.CLI.Extract;
using VortexEngine.CLI.Pack;

namespace VortexEngine.CLI;

public class VortexEngineCLI
{
    static int Main(string[] args)
    {
        VortexEngineCLI cli = new VortexEngineCLI(args);
        return cli.Execute(args);
    }

    public VortexEngineCLI(string[] args)
    {
        Log.InitLogger();
    }

    public int Execute(string[] args)
    {
        return Parser.Default.ParseArguments<PackCommand.Options, ExtractCommand.Options>(args)
            .MapResult<PackCommand.Options, ExtractCommand.Options, int>(
                PackCommand.Run,
                ExtractCommand.Run,
                errs => 1);
    }
}