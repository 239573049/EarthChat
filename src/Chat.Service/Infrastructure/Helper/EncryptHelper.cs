using System.Security.Cryptography;
using System.Text;

namespace Chat.Service.Infrastructure.Helper;

public static class EncryptHelper
{
    private static string key;

    public static IServiceProvider UseEncrypt(this IServiceProvider services)
    {
        var configuration = services.GetRequiredService<IConfiguration>();
        key = configuration["Key"]?.Trim();

        if (key.IsNullOrWhiteSpace())
        {
            throw new ArgumentNullException("您的 Encrypt的 Key 并未配置！");
        }

        return services;
    }

    /// <summary>
    /// 加密
    /// </summary>
    /// <param name="pToEncrypt"></param>
    /// <returns></returns>
    public static string Encrypt(string pToEncrypt)
    {
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
        var des = new DESCryptoServiceProvider();
        byte[] inputByteArray = new byte[pToDecrypt.Length / 2];
        for (int x = 0; x < pToDecrypt.Length / 2; x++)
        {
            int i = (Convert.ToInt32(pToDecrypt.Substring(x * 2, 2), 16));
            inputByteArray[x] = (byte)i;
        }

        des.Key = Encoding.ASCII.GetBytes(key);
        des.IV = Encoding.ASCII.GetBytes(key);
         MemoryStream ms = new MemoryStream();
         CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write);
        cs.Write(inputByteArray, 0, inputByteArray.Length);
        cs.FlushFinalBlock();
        return Encoding.Default.GetString(ms.ToArray());
    }
}