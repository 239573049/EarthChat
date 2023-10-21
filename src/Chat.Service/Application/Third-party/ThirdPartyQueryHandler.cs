using Chat.Contracts.third_party;
using Chat.Service.Application.Third_party.Queries;

namespace Chat.Service.Application.Third_party;

public class ThirdPartyQueryHandler
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<ThirdPartyQueryHandler> _logger;

    public ThirdPartyQueryHandler(IHttpClientFactory httpClientFactory, ILogger<ThirdPartyQueryHandler> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    [EventHandler]
    public async Task GetObtainingIpHome(GetObtainingIPHomeQuery query)
    {
        try
        {
            var http = _httpClientFactory.CreateClient(nameof(ThirdPartyQueryHandler));
            query.Result =
                await http.GetFromJsonAsync<GetObtainingIPHomeDto>(
                    "https://whois.pconline.com.cn/ipJson.jsp?json=true&ip=" + query.ip);
        }
        catch (Exception e)
        {
            _logger.LogError("获取ip归属错误：{e}", e);
        }
    }
}