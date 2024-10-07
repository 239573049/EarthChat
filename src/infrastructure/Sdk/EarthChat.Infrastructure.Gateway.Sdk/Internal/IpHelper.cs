using System.Net;
using System.Net.Sockets;

namespace EarthChat.Infrastructure.Gateway.Sdk.Internal;

public class IpHelper
{
    // 获取本地IP
    public static string GetLocalIp()
    {
        // 获取ip，过滤1后缀的ip，127
        var hostName = Dns.GetHostName();
        var ipEntry = Dns.GetHostEntry(hostName);
        foreach (var ip in ipEntry.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork && !ip.ToString().StartsWith("127") &&
                !ip.ToString().EndsWith(".1"))
            {
                return ip.ToString();
            }
        }

        return string.Empty;
    }
}