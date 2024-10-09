using System.Net;
using System.Net.Sockets;

namespace EarthChat.Gateway.Sdk.Internal;

/// <summary>
///     Ip帮助类
/// </summary>
public static class IpHelper
{
    // 获取本地IP
    public static string GetLocalIp()
    {
        // 获取ip，过滤1后缀的ip，127
        var ipEntry = Dns.GetHostEntry(Dns.GetHostName());
        foreach (var ip in ipEntry.AddressList)
            if (ip.AddressFamily == AddressFamily.InterNetwork && !ip.ToString().EndsWith(".1"))
                return ip.ToString();

        return string.Empty;
    }
}