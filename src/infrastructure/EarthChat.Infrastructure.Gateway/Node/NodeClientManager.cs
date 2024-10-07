using System.Collections.Concurrent;

namespace EarthChat.Infrastructure.Gateway.Node;

public class NodeClientManager
{
    private readonly ConcurrentDictionary<string, ConcurrentDictionary<string, NodeClient>> _nodeClients = new();

    public void Add(NodeClient nodeClient)
    {
        if (!_nodeClients.TryGetValue(nodeClient.Service.ToLower(), out var nodeClients))
        {
            nodeClients = new ConcurrentDictionary<string, NodeClient>();
            _nodeClients.TryAdd(nodeClient.Service.ToLower(), nodeClients);
        }

        nodeClients.TryAdd(nodeClient.Key.ToLower(), nodeClient);
    }

    public List<NodeClient> Get(string key)
    {
        return _nodeClients.TryGetValue(key.ToLower(), out var nodeClients)
            ? nodeClients.Values.ToList()
            : [];
    }

    public void Remove(string service, string key)
    {
        if (!_nodeClients.TryGetValue(service.ToLower(), out var nodeClients)) return;
        
        nodeClients.TryRemove(key.ToLower(), out _);

        if (nodeClients.IsEmpty)
        {
            _nodeClients.TryRemove(service.ToLower(), out _);
        }
    }

    public IEnumerable<List<NodeClient>> GetAll()
    {
        return _nodeClients.Values.Select(nodeClients => nodeClients.Values.ToList());
    }

    public void Clear()
    {
        _nodeClients.Clear();
    }

    public bool Contains(string key)
    {
        return _nodeClients.ContainsKey(key.ToLower());
    }

    public bool Contains(NodeClient nodeClient)
    {
        return _nodeClients.ContainsKey(nodeClient.Key.ToLower());
    }
}