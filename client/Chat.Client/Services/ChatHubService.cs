using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;

namespace Chat.Client.Services;

public class ChatHubService
{
    private bool _isConnected;

    private HubConnection? _hubConnection;

    public Action<ChatMessageDto> _receiveMessage;

    /// <summary>
    /// 重新连接次数
    /// </summary>
    private int _reconnectCount = 5;

    public async Task StartAsync()
    {
        _hubConnection = new HubConnectionBuilder()
            .WithUrl("https://chat.tokengo.top/api/chathHub", options =>
            {
                options.AccessTokenProvider = async () =>
                {
                    var storageService = MainAppHelper.GetRequiredService<StorageService>();
                    return await Task.FromResult(storageService.GetToken());
                };
            })
            .AddMessagePackProtocol()
            .Build();

        await _hubConnection.StartAsync();

        _isConnected = true;

        // 断线重连
        _hubConnection.Closed += async (error) =>
        {
            _isConnected = false;
            _reconnectCount--;

            if (_reconnectCount <= 0)
            {
                return;
            }

            await Task.Delay(1000);

            await _hubConnection.StartAsync();
            _isConnected = true;
        };

        // 接收消息.
        _hubConnection.On<string>("ReceiveMessage", (message) =>
        {
            var dto = JsonSerializer.Deserialize<ChatMessageDto>(message, new JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = true
            });

            _receiveMessage?.Invoke(dto);
        });
        
    }

    public async Task SendMessageAsync(string value,string groupId, ChatType type)
    {
        if (!_isConnected)
        {
            await StartAsync();
        }

        await _hubConnection?.SendAsync("SendMessage", value, groupId,type)!;
    }

    public async Task StopAsync()
    {
        await _hubConnection?.StopAsync()!;
    }
}