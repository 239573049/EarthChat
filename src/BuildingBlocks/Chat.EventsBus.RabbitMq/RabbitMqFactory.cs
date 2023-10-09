using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace Chat.EventsBus.RabbitMq;

public class RabbitMqFactory : IDisposable
{
    private readonly RabbitOptions _options;
    private readonly ConnectionFactory _factory;
    private IConnection? _connection;

    public RabbitMqFactory(IOptions<RabbitOptions> options)
    {
        _options = options?.Value;
        // 将Options中的参数添加到ConnectionFactory
        _factory = new ConnectionFactory
        {
            HostName = _options.HostName,
            UserName = _options.UserName,
            Password = _options.Password,
            Port = _options.Port
        };
    }

    public IModel CreateRabbitMQ()
    {
        // 当第一次创建RabbitMQ的时候进行链接
        _connection ??= _factory.CreateConnection();

        return _connection.CreateModel();
    }

    public void Dispose()
    {
        _connection?.Dispose();
    }
}