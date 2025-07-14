using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using VortexEngine.Rendering;

namespace VortexEngine.Internal.AssetManagement;

public static class AssetManager
{

    private static Dictionary<string, Texture> TextureDatabase = new Dictionary<string, Texture>();

    public static List<string> AvailableImageIds = new List<string>();

    public static List<string> AvailablePrefabIds = new List<string>();

    private static CipherType AssetDatabaseCipher = CipherType.Base64 | CipherType.Shift | CipherType.Swap;

    private static AssetDatabaseFile database = new AssetDatabaseFile();


    public static AssetDatabaseFile GetDatabase()
    {
        return database;
    }
    
    public static Texture GetTexture(string id)
    {
        if (!TextureDatabase.ContainsKey(id))
        {
            Texture img = Renderer.backend.LoadTexture(VortexEngine.ProjectDataPath + GetEncodedID(id + ".png"));

            Renderer.backend.TextureFlipVertical(img);

            TextureDatabase.Add(id, img);
            Console.WriteLine(VortexEngine.ProjectDataPath + id + ".png");
        }

        return TextureDatabase[id];

    }


    public static void LoadFromFile(string data)
    {
        database = AssetDatabaseFile.Deserialize(data, AssetDatabaseCipher);
    }
    
    public static AssetDatabaseFile LoadDatabaseFile(string data)
    {
        return AssetDatabaseFile.Deserialize(data, AssetDatabaseCipher);
    }
    
    public static void AddData(string folder, bool overrideInEditor = true)
    {
        database.FromFolder(folder, AssetDatabaseCipher, overrideInEditor);
    }

    public static void AddFile(byte[] contents, string id, UnixFileMode mode = UnixFileMode.None)
    {
        List<AssetDatabaseEntry> entries = database.Entries.ToList();

        
        entries.Add(new AssetDatabaseEntry(){data = contents, path = id, mode = mode});

        database.Entries = entries.ToArray();
    }
    
    public static string SerializeDatabase()
    {
        return database.Serialize(AssetDatabaseCipher);
    }
    
    public static byte[] RawSerializeDatabase()
    {
        return database.RawSerialize();
    }
 
    public static T? LoadData<T>(string path)
    {
        return JsonSerializer.Deserialize<T>(ReadAllText(path + ".json"), new JsonSerializerOptions{ IncludeFields = true, WriteIndented = true });
    }

    public static void SaveData<T>(T data, string path)
    {


        File.WriteAllText(VortexEngine.ProjectDataPath + path + ".json", JsonSerializer.Serialize<T>(data, new JsonSerializerOptions{ IncludeFields = true, WriteIndented = true }));

    }

    public static string ReadAllText(string id)
    {
        if (VortexEngine.InEditor){
            Console.WriteLine("Loading asset from: " + VortexEngine.ProjectDataPath + GetEncodedID(id));
            return File.ReadAllText(VortexEngine.ProjectDataPath + id);
        } else {
            Console.WriteLine("Loading asset from: " + VortexEngine.ProjectDataPath + GetEncodedID(id));

            if (database == null)
            {
                Log.Fatal("Database file not loaded.");
            }
            
            return Encoding.Default.GetString(database.GetEntry(GetEncodedID(id), AssetDatabaseCipher).data);
        }
    }
    
    public static byte[] ReadAllBytes(string id, bool ignoreEncryption = false)
    {
        if (VortexEngine.InEditor){
            Console.WriteLine("Loading asset from: " + VortexEngine.ProjectDataPath + GetEncodedID(id));
            return File.ReadAllBytes(VortexEngine.ProjectDataPath + id);
        } else {

            if (ignoreEncryption)
            {
                Console.WriteLine("Loading asset from: " + VortexEngine.ProjectDataPath + id);

                if (database == null)
                {
                    Log.Fatal("Database file not loaded.");
                }
            
                return database.GetEntry(id, AssetDatabaseCipher).data;
            }
            else
            {
                Console.WriteLine("Loading asset from: " + VortexEngine.ProjectDataPath + GetEncodedID(id));

                if (database == null)
                {
                    Log.Fatal("Database file not loaded.");
                }
            
                return database.GetEntry(GetEncodedID(id), AssetDatabaseCipher).data;
            }
            
        }
    }

    public static string GetEncodedID(string id, bool overrideInEditor = false){
        string[] items = id.Split("/");
        string extension = items.Last().Split(".").Last();
        string name = items.Last().Split(".")[0];
        List<string> folders = items.ToList();
        folders.ToList().RemoveAt(folders.Count - 1);

        List<string> EncodedFolders = new List<string>();

        string jointFolders = "";

        foreach (var item in folders)
        {

            if (item.Split(".")[0] != name){
                Console.WriteLine(item.Split(".")[0] + " != " + name);
                EncodedFolders.Add(Sha256(item));
                jointFolders += Sha256(item) + "/";
            } else {
                Console.WriteLine(item.Split(".")[0] + " == " + name);
            }

        }


        string encodedpath = jointFolders + Sha256(name + "." + extension);

        Console.WriteLine(encodedpath);

        if (VortexEngine.InEditor && !overrideInEditor){
            return id;
        } else {

            return encodedpath;
        }


    }

    public static string Sha256(string plainText)
    {
        using (SHA256 sha256 = SHA256.Create())
        {
            return Convert.ToBase64String(sha256.ComputeHash(Encoding.UTF8.GetBytes(plainText)));
        }
    }

    public static string DataEncrypt(string plainText)
    {
        return DataEncryption.DataEncrypt(plainText, AssetDatabaseCipher);
    }

    public static string DataDecrypt(string data)
    {
        return DataEncryption.DataDecrypt(data, AssetDatabaseCipher);
    }

}
