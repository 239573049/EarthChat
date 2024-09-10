using System.Net.Http.Json;
using EarthChat.Core.Models;
using EarthChat.Infrastructure.Gateway.Node;
using EarthChat.Infrastructure.Gateway.Sdk.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace EarthChat.Infrastructure.Gateway.Sdk.Internal.Client;

public class GatewayClient : IGatewayClient
{
    private readonly HttpClient _client;
    private readonly GatewayServiceOptions _options;
    private readonly ILogger<GatewayClient> _logger;
    private string _id;

    public GatewayClient(IHttpClientFactory httpClientFactory, IOptions<GatewayServiceOptions> options,
        ILogger<GatewayClient> logger)
    {
        _logger = logger;
        _options = options.Value;
        _client = httpClientFactory.CreateClient(Const.HttpClientName);
    }

    public async Task RegisterAsync(CancellationToken cancellationToken)
    {
        // 发送注册请求
        var response = await _client.PostAsJsonAsync("/api/v1/node/register", new NodeClient()
        {
            Service = _options.Service,
            Token = _options.Token,
            Ip = _options.Ip,
            Port = _options.Port,
        }, cancellationToken);

        response.EnsureSuccessStatusCode();

        // 注册成功
        var result = await response.Content.ReadFromJsonAsync<ResultDto<string>>(cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        if (result is not { Success: true })
        {
            throw new Exception(result?.Message);
        }

        _logger.LogInformation("服务注册成功 id:{id}", result.Data);

        _id = result.Data ?? throw new Exception("注册失败 id 为空");
    }

    public Task ServiceDeregisterAsync(CancellationToken cancellationToken)
    {
        return _client.DeleteAsync("/api/v1/node/deregister?key=" + _id, cancellationToken);
    }
}