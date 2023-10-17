using System.Threading.Channels;
using Chat.Contracts.Chats;
using Chat.Contracts.Eto.Semantic;

namespace Chat.Service.Services;

/// <summary>
/// 后台任务。
/// </summary>
public class BackgroundTaskService : ISingletonDependency, IDisposable
{
    /// <summary>
    /// 消息管道。
    /// </summary>
    private readonly Channel<AssistantDto> _channel;

    private readonly IServiceScope _serviceScope;

    private readonly CancellationTokenSource _cancellationTokenSource;

    private readonly RedisClient _redisClient;

    public BackgroundTaskService(IServiceProvider serviceProvider, RedisClient redisClient)
    {
        _redisClient = redisClient;
        // 请注意由于仓储使用的是作用域，所以需要在这个单例的服务中创建一个作用域。
        _serviceScope = serviceProvider.CreateScope();
        // 使用管道的话会控制ChatGPT的数量，相当于当前系统只运行一个服务。
        _channel = Channel.CreateBounded<AssistantDto>(5000);
        _cancellationTokenSource = new CancellationTokenSource();
        Task.Run(Start);
    }

    private async Task Start()
    {
        while (!_cancellationTokenSource.IsCancellationRequested)
        {
            var item = await _channel.Reader.ReadAsync(_cancellationTokenSource.Token);
            if (item != null)
            {
                // 触发聊天机器人
                if (!item.Value.StartsWith("ai") || item.Value.Length <= 3) continue;

                string value = item.Value[2..].Trim();

                if (value.IsNullOrWhiteSpace())
                {
                    continue;
                }

                await _redisClient.PublishAsync(nameof(IntelligentAssistantEto), JsonSerializer.Serialize(
                    new IntelligentAssistantEto()
                    {
                        Group = item.Group,
                        Id = item.Id,
                        RevertId = item.RevertId,
                        UserId = item.UserId,
                        Value = item.Value
                    }));

            }
        }
    }

    /// <summary>
    /// 写入到管道。
    /// </summary>
    /// <param name="dto"></param>
    public async Task WriteAsync(AssistantDto dto)
    {
        await _channel.Writer.WriteAsync(dto);
    }

    public void Dispose()
    {
        _cancellationTokenSource.Cancel();
        _serviceScope.Dispose();
    }
}