using System.Security.Cryptography;
using System.Text;

namespace Chat.Service.Infrastructure.Helper;

public static class EncryptHelper
{
    private static string key = "s!9%h_h,";

    /// <summary>
    /// 加密
    /// </summary>
    /// <param name="pToEncrypt"></param>
    /// <returns></returns>
    public static string Encrypt(string pToEncrypt)
    {
        if (pToEncrypt.IsNullOrWhiteSpace())
        {
            return string.Empty;
        }

        var des = new DESCryptoServiceProvider();
        byte[] inputByteArray = Encoding.Default.GetBytes(pToEncrypt);
        des.Key = Encoding.ASCII.GetBytes(key);
        des.IV = Encoding.ASCII.GetBytes(key);
        using MemoryStream ms = new MemoryStream();
        using CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write);
        cs.Write(inputByteArray, 0, inputByteArray.Length);
        cs.FlushFinalBlock();
        StringBuilder ret = new StringBuilder();
        foreach (var b in ms.ToArray())
        {
            ret.AppendFormat("{0:X2}", b);
        }

        return ret.ToString();
    }

    /// <summary>
    /// 解密
    /// </summary>
    /// <param name="pToDecrypt"></param>
    /// <returns></returns>
    public static string Decrypt(string pToDecrypt)
    {
        if (pToDecrypt.IsNullOrWhiteSpace())
        {
            return string.Empty;
        }

        var des = new DESCryptoServiceProvider();
        byte[] inputByteArray = new byte[pToDecrypt.Length / 2];
        for (int x = 0; x < pToDecrypt.Length / 2; x++)
        {
            int i = (Convert.ToInt32(pToDecrypt.Substring(x * 2, 2), 16));
            inputByteArray[x] = (byte)i;
        }

        des.Key = Encoding.ASCII.GetBytes(key);
        des.IV = Encoding.ASCII.GetBytes(key);
        using var ms = new MemoryStream();
        using var cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write);
        cs.Write(inputByteArray, 0, inputByteArray.Length);
        cs.FlushFinalBlock();
        return Encoding.Default.GetString(ms.ToArray());
    }
}