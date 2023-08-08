using System.Collections.Generic;
using System.Linq;

namespace Chat.Client;

public interface IEventBus
{
    void Publish(string key, object @event);

    void Subscribe(string key, Action<object> action);

    void UnSubscribe(string key, Action<object> action);
}

public class EventBus : IEventBus
{
    private readonly Dictionary<string, Action<object>> _actions = new();

    public void Publish(string key, object @event)
    {
        var values = _actions.Where(x => x.Key == key);

        foreach (var value in values)
        {
            value.Value.Invoke(@event);
        }
    }

    public void Subscribe(string key, Action<object> action)
    {
        _actions.Add(key, action);
    }

    public void UnSubscribe(string key, Action<object> action)
    {
        _actions.Remove(key);
    }
}