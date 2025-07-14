using System;
using System.IO;
using System.Text;

namespace VortexEngine.Editor;

public static class ExePacker
{
    private const string MagicFooter = "ASSETDATABASE";

    public static void PackFileIntoExe(string exePath, string fileToPackPath, string outputExePath)
    {
        byte[] exeBytes = File.ReadAllBytes(exePath);
        byte[] fileBytes = File.ReadAllBytes(fileToPackPath);
        byte[] fileNameBytes = Encoding.UTF8.GetBytes(Path.GetFileName(fileToPackPath));
        int fileNameLength = fileNameBytes.Length;
        int fileSize = fileBytes.Length;

        using var output = new FileStream(outputExePath, FileMode.Create, FileAccess.Write);
        using var writer = new BinaryWriter(output);

        writer.Write(exeBytes);

        writer.Write(fileBytes);

        writer.Write(fileNameBytes);
        writer.Write(fileSize);
        writer.Write(fileNameLength);

        writer.Write(Encoding.ASCII.GetBytes(MagicFooter));
    }
}
