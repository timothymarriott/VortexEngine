using System.Security.Cryptography;
using System.Text;

namespace VortexEngine.Internal.AssetManagement;

[Flags]
public enum CipherType
{
    None = 0,
    Base64 = 1,
    AES = 2,
    TripleDES = 4,
    Shift = 8,
    Swap = 16
}

public static class DataEncryption
{
    //Eventually these keys could be grabbed via a server or defined by the developer when building a project
    //The current cipher options done use AES or DES
    private static readonly byte[] AesKey = Encoding.UTF8.GetBytes("aes_key_2025_123");
    private static readonly byte[] AesIV = Encoding.UTF8.GetBytes("aes_iv_2025_321");

    private static readonly byte[] TripleDesKey = Encoding.UTF8.GetBytes("des_key_2025_123");
    private static readonly byte[] TripleDesIV = Encoding.UTF8.GetBytes("des_iv_2025_321");

    public static string DataEncrypt(string plainText, CipherType cipherType)
    {
        byte[] dataBytes = Encoding.UTF8.GetBytes(plainText);

        if (cipherType.HasFlag(CipherType.Shift))
        {
            dataBytes = EncryptShift(dataBytes, 3);
        }

        if (cipherType.HasFlag(CipherType.Swap))
        {
            dataBytes = EncryptSwap(dataBytes);
        }

        if (cipherType.HasFlag(CipherType.AES))
        {
            dataBytes = EncryptAes(dataBytes);
        }

        if (cipherType.HasFlag(CipherType.TripleDES))
        {
            dataBytes = EncryptTripleDes(dataBytes);
        }

        if (cipherType.HasFlag(CipherType.Base64))
        {
            return Convert.ToBase64String(dataBytes);
        }

        return Encoding.UTF8.GetString(dataBytes);
    }

    public static string DataDecrypt(string data, CipherType cipherType)
    {
        byte[] dataBytes = cipherType.HasFlag(CipherType.Base64)
            ? Convert.FromBase64String(data)
            : Encoding.UTF8.GetBytes(data);

        if (cipherType.HasFlag(CipherType.TripleDES))
        {
            dataBytes = DecryptTripleDes(dataBytes);
        }

        if (cipherType.HasFlag(CipherType.AES))
        {
            dataBytes = DecryptAes(dataBytes);
        }

        if (cipherType.HasFlag(CipherType.Swap))
        {
            dataBytes = DecryptSwap(dataBytes);
        }

        if (cipherType.HasFlag(CipherType.Shift))
        {
            dataBytes = DecryptShift(dataBytes, 3);
        }

        return Encoding.UTF8.GetString(dataBytes);
    }

    private static byte[] EncryptAes(byte[] plainTextBytes)
    {
        using var aes = Aes.Create();
        aes.Key = AesKey;
        aes.IV = AesIV;

        using var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
        return encryptor.TransformFinalBlock(plainTextBytes, 0, plainTextBytes.Length);
    }

    private static byte[] DecryptAes(byte[] encryptedBytes)
    {
        using var aes = Aes.Create();
        aes.Key = AesKey;
        aes.IV = AesIV;

        using var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
        return decryptor.TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length);
    }

    private static byte[] EncryptTripleDes(byte[] plainTextBytes)
    {
        using var tripleDes = TripleDES.Create();
        tripleDes.Key = TripleDesKey;
        tripleDes.IV = TripleDesIV;

        using var encryptor = tripleDes.CreateEncryptor(tripleDes.Key, tripleDes.IV);
        return encryptor.TransformFinalBlock(plainTextBytes, 0, plainTextBytes.Length);
    }

    private static byte[] DecryptTripleDes(byte[] encryptedBytes)
    {
        using var tripleDes = TripleDES.Create();
        tripleDes.Key = TripleDesKey;
        tripleDes.IV = TripleDesIV;

        using var decryptor = tripleDes.CreateDecryptor(tripleDes.Key, tripleDes.IV);
        return decryptor.TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length);
    }

    private static byte[] EncryptShift(byte[] data, int shift)
    {
        byte[] result = new byte[data.Length];
        for (int i = 0; i < data.Length; i++)
        {
            result[i] = (byte)(data[i] + shift);
        }
        return result;
    }

    private static byte[] DecryptShift(byte[] data, int shift)
    {
        byte[] result = new byte[data.Length];
        for (int i = 0; i < data.Length; i++)
        {
            result[i] = (byte)(data[i] - shift);
        }
        return result;
    }
    
    private static byte[] EncryptSwap(byte[] data)
    {
        byte[] result = new byte[data.Length];
        Array.Copy(data, result, data.Length);

        for (int i = 0; i < result.Length - 1; i += 2)
        {
            (result[i], result[i + 1]) = (result[i + 1], result[i]);
        }
        return result;
    }

    private static byte[] DecryptSwap(byte[] data)
    {
        return EncryptSwap(data);
    }
}
