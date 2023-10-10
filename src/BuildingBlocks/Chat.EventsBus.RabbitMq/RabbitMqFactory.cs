using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace Chat.EventsBus.RabbitMq;

public class RabbitMqFactory : IDisposable
{
    private readonly ConnectionFactory _factory;
    private IConnection? _connection;

    public RabbitMqFactory(IOptions<RabbitOptions> options)
    {
        var options1 = options?.Value;
        // 将Options中的参数添加到ConnectionFactory
        _factory = new ConnectionFactory
        {
            HostName = options1!.HostName,
            UserName = options1.UserName,
            Password = options1.Password,
            Port = options1.Port
        };
    }

    public IModel CreateRabbitMq()
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