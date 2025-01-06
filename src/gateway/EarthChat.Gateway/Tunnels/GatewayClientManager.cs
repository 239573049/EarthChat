using System.Collections;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace EarthChat.Infrastructure.Gateway.Tunnels;

/// <summary>
/// 客户端管理器
/// </summary>
[DebuggerDisplay("Count = {Count}")]
public sealed class GatewayClientManager : IEnumerable
{
    private readonly ConcurrentDictionary<string, ConcurrentDictionary<string, GatewayClient>> _dictionary = new();

    private readonly GatewayClientStateChannel clientStateChannel;

    public GatewayClientManager(GatewayClientStateChannel clientStateChannel)
    {
        this.clientStateChannel = clientStateChannel;
    }

    /// <inheritdoc/>
    public int Count => this._dictionary.Count;


    /// <inheritdoc/>
    public bool TryGetValue(string clientId,
        [MaybeNullWhen(false)] out ConcurrentDictionary<string, GatewayClient> client)
    {
        return this._dictionary.TryGetValue(clientId, out client);
    }

    /// <summary>
    /// 添加客户端实例
    /// </summary>
    /// <param name="client">客户端实例</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async ValueTask<bool> AddAsync(GatewayClient client, CancellationToken cancellationToken)
    {
        var clients = _dictionary.GetOrAdd(client.NodeName, _ => new ConcurrentDictionary<string, GatewayClient>());

        if (!clients.TryAdd(client.Id, client)) return false;
        await this.clientStateChannel.WriteAsync(client, connected: true, cancellationToken);
        return true;
    }

    /// <summary>
    /// 移除客户端实例
    /// </summary>
    /// <param name="client">客户端实例</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async ValueTask<bool> RemoveAsync(GatewayClient client, CancellationToken cancellationToken)
    {
        if (_dictionary.TryGetValue(client.NodeName, out var clients))
        {
            if (clients.TryRemove(client.Id, out var existClient))
            {
                if (ReferenceEquals(existClient, client))
                {
                    await this.clientStateChannel.WriteAsync(client, connected: false, cancellationToken);
                    return true;
                }
                else
                {
                    clients.TryAdd(client.Id, existClient);
                }
            }
        }

        return false;
    }


    /// <inheritdoc/>
    private IEnumerator<GatewayClient> GetEnumerator()
    {
        return _dictionary.SelectMany(keyValue => keyValue.Value.Values).GetEnumerator();
    }

    public (string, ConcurrentDictionary<string, GatewayClient>)[] ToArray()
    {
        return _dictionary.Select(x => (x.Key, x.Value)).ToArray();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return this.GetEnumerator();
    }
}