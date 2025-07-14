using System.Text;
using System.Text.Json;
using ProtoBuf;

namespace VortexEngine.Internal.AssetManagement;

[ProtoContract]
public class AssetDatabaseFile
{

    public static bool UseCustomRoot;
    public static string CustomRoot = "";

    [ProtoMember(1)] public AssetDatabaseEntry[] Entries { get; set; } = new AssetDatabaseEntry[0];

    public AssetDatabaseEntry GetEntry(string path, CipherType cipher)
    {
        
        foreach (var entry in Entries)
        {
            if (entry.path == path)
            {
                return entry;
            }
        }

        Log.Fatal($"Databse entry \"{path}\" not found.");
        return null;
    }

    public string Serialize(CipherType cipher)
    {
        string text = JsonSerializer.Serialize(this);

        byte[] data = Encoding.UTF8.GetBytes(text);
        string b64 = Convert.ToBase64String(data);
        string encrypted = DataEncryption.DataEncrypt(b64, cipher);

        return encrypted;
    }

    public byte[] RawSerialize()
    {
        MemoryStream stream = new MemoryStream();
        JsonSerializer.Serialize(stream, this);

        return stream.GetBuffer();
    }
    public static AssetDatabaseFile Deserialize(string data, CipherType cipher)
    {
        byte[] binaryData = Convert.FromBase64String(DataEncryption.DataDecrypt(data, cipher));
        return JsonSerializer.Deserialize<AssetDatabaseFile>(Encoding.UTF8.GetString(binaryData));
    }

    public void FromFolder(string folder, CipherType cipher, bool overrideInEditor = true)
    {
        if (!Directory.Exists(folder))
        {
            return;
        }
        
        List<AssetDatabaseEntry> entries = new List<AssetDatabaseEntry>();
        
        foreach (var assetDatabaseEntry in Entries)
        {
            entries.Add(assetDatabaseEntry);
        }

        foreach (string file in Directory.EnumerateFiles(folder, "*.*", SearchOption.AllDirectories))
        {

            if (new FileInfo(file).Name == ".DS_Store")
            {
                continue;
            }
            string id = file.Replace(UseCustomRoot ? CustomRoot : VortexEngine.ProjectDataPath, "");
            string path = AssetManager.GetEncodedID(id, overrideInEditor);
            AssetDatabaseEntry? toRemove = null;
            foreach (var assetDatabaseEntry in Entries)
            {
                if (assetDatabaseEntry.path == path)
                {
                    toRemove = assetDatabaseEntry;
                }
            }

            if (toRemove != null)
            {
                entries.Remove(toRemove);
                Log.Warning($"Replacing \"{id}\"");
            }
            else
            {
                Log.Info($"Including \"{id}\"");
            }


            UnixFileMode mode = UnixFileMode.None;
            if (!OperatingSystem.IsWindows())
            {
                mode = File.GetUnixFileMode(file);
            }
            entries.Add(new AssetDatabaseEntry()
            {
                data = File.ReadAllBytes(file),
                path = path,
                mode = mode
            });
        }
        Entries = entries.ToArray();

    }

    public void Extract(string targetDir)
    {
        foreach (var entry in Entries)
        {
            if (!Directory.Exists(new FileInfo(Path.Join(targetDir, entry.path)).DirectoryName))
                Directory.CreateDirectory(new FileInfo(Path.Join(targetDir, entry.path)).DirectoryName);
            
            File.WriteAllBytes(Path.Join(targetDir, entry.path), entry.data);
            if(!OperatingSystem.IsWindows())
                File.SetUnixFileMode(Path.Join(targetDir, entry.path), entry.mode);
            
        }
    }
}

[ProtoContract]
public class AssetDatabaseEntry
{
    [ProtoMember(1)] public byte[] data { get; set; } = new byte[0];
    [ProtoMember(2)] public string path { get; set; } = "";

    [ProtoMember(3)] public UnixFileMode mode { get; set; } = UnixFileMode.None;
}