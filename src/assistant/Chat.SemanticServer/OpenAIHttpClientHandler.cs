using Chat.SemanticServer.Options;

namespace Chat.SemanticServer;

public class OpenAIHttpClientHandler : HttpClientHandler
{
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var uriBuilder = new UriBuilder(OpenAIOptions.Endpoint.TrimEnd("/") + request.RequestUri.LocalPath);
        request.RequestUri = uriBuilder.Uri;
        return base.SendAsync(request, cancellationToken);
    }
}