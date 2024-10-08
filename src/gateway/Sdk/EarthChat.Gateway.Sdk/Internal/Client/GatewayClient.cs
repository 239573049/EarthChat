﻿using System.Net.Http.Json;
using EarthChat.Core.Models;
using EarthChat.Core.System.Extensions;
using EarthChat.Gateway.Sdk.Options;
using EarthChat.Infrastructure.Gateway.Node;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace EarthChat.Gateway.Sdk.Internal.Client;

public class GatewayClient : IGatewayClient
{
    private readonly HttpClient _client;
    private readonly ILogger<GatewayClient> _logger;
    private readonly GatewayServiceOptions _options;
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
        if (_options.Ip.IsNullOrWhiteSpace()) _options.Ip = IpHelper.GetLocalIp();

        // 发送注册请求
        var response = await _client.PostAsJsonAsync("/api/v1/node/register", new NodeClient
        {
            Service = _options.Service,
            Token = _options.Token,
            Ip = _options.Ip,
            Port = _options.Port,
            HealthCheck = _options.HealthCheck
        }, cancellationToken);

        // 注册成功
        var result = await response.Content.ReadFromJsonAsync<ResultDto<string>>(cancellationToken)
            .ConfigureAwait(false);

        if (result is not { Success: true }) throw new Exception(result?.Message);

        _logger.LogInformation("服务注册成功 id:{id}", result.Data);

        _id = result.Data ?? throw new Exception("注册失败 id 为空");

        try
        {
            await InspectService(cancellationToken);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "服务检查失败");
        }
    }

    public Task ServiceDeregisterAsync(CancellationToken cancellationToken)
    {
        return _client.DeleteAsync("/api/v1/node/deregister?key=" + _id, cancellationToken);
    }

    private async Task InspectService(CancellationToken cancellationToken)
    {
        // 注册成功后，启动服务检查
        while (!cancellationToken.IsCancellationRequested)
            try
            {
                // 更新状态给网关
                await _client.PostAsJsonAsync("/api/v1/node/inspect", new NodeClient
                {
                    Key = _id,
                    Service = _options.Service,
                    Token = _options.Token,
                    Ip = _options.Ip,
                    Port = _options.Port,
                    HealthCheck = _options.HealthCheck
                }, cancellationToken);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "服务检查失败");
            }
            finally
            {
                await Task.Delay(15000, cancellationToken);
            }
    }
}