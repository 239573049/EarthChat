using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;

namespace Chat.Client.Services;

public class ChatHubService
{
    private bool _isConnected;

    private HubConnection? _hubConnection;

    /// <summary>
    /// 重新连接次数
    /// </summary>
    private int _reconnectCount = 5;

    public async Task StartAsync()
    {
        _hubConnection = new HubConnectionBuilder()
            .WithUrl("/chatHub", options =>
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
            
        });
    }
    
    
}