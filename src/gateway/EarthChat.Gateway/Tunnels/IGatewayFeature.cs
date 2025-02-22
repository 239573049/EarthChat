﻿using EarthChat.Gateway.Sdk.Data;

namespace EarthChat.Infrastructure.Gateway.Tunnels;

public interface IGatewayFeature
{
    /// <summary>
    /// 获取当前是否为隧道请求
    /// </summary>
    bool IsRequest { get; }

    /// <summary>
    /// 获取传输协议类型
    /// </summary>
    TransportProtocol Protocol { get; }

    /// <summary>
    /// 使用当前物理连接创建新的双工流
    /// </summary>
    /// <returns></returns>
    Task<Stream> AcceptAsStreamAsync();

    /// <summary>
    /// 使用当前物理连接创建新的线程安全写入的双工流
    /// </summary>
    /// <returns></returns>
    Task<Stream> AcceptAsSafeWriteStreamAsync();
}