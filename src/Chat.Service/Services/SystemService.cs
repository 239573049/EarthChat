using System.Security.Cryptography;
using System.Text;

namespace Chat.Service.Services;

public class SystemService
{
    private readonly IConfiguration _configuration;

    public SystemService(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    /// <summary>
    /// 加密
    /// </summary>
    /// <param name="pToEncrypt"></param>
    /// <returns></returns>
    public string Encrypt(string pToEncrypt)
    {
        var des = new DESCryptoServiceProvider();
        byte[] inputByteArray = Encoding.Default.GetBytes(pToEncrypt);
        des.Key = Encoding.ASCII.GetBytes(_configuration["Key"]);
        des.IV = Encoding.ASCII.GetBytes(_configuration["Key"]);
        MemoryStream ms = new MemoryStream();
        CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write);
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
    public string Decrypt(string pToDecrypt)
    {
        var des = new DESCryptoServiceProvider();
        byte[] inputByteArray = new byte[pToDecrypt.Length / 2];
        for (int x = 0; x < pToDecrypt.Length / 2; x++)
        {
            int i = (Convert.ToInt32(pToDecrypt.Substring(x * 2, 2), 16));
            inputByteArray[x] = (byte)i;
        }
        des.Key = Encoding.ASCII.GetBytes(_configuration["Key"]);
        des.IV = Encoding.ASCII.GetBytes(_configuration["Key"]);
        MemoryStream ms = new MemoryStream();
        CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write);
        cs.Write(inputByteArray, 0, inputByteArray.Length);
        cs.FlushFinalBlock();
        return Encoding.Default.GetString(ms.ToArray());
    }
}