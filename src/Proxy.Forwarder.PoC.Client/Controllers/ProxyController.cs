namespace Proxy.Forwarder.PoC.Client.Controllers;

using Microsoft.AspNetCore.Mvc;
using Proxy.Forwarder.PoC.Client.Nginx;
using Proxy.Forwarder.PoC.Client.ValueModels;
using Proxy.Forwarder.PoC.Client.Yarp;

[ApiController, Route("[controller]/[action]")]
public sealed class ProxyController : ControllerBase
{
    private const string WEBHOOK_URL_HEADER = "WebhookUrl";

    private readonly HttpClient nginxClient;
    private readonly HttpClient yarpClient;

    public ProxyController(IHttpClientFactory httpClientFactory)
    {
        this.nginxClient = httpClientFactory.CreateClient(NginxOptions.HTTP_CLIENT_NAME);
        this.yarpClient = httpClientFactory.CreateClient(YarpOptions.HTTP_CLIENT_NAME);
    }

    [HttpPost(Name = "Send/Nginx")]
    public async Task<IActionResult> SendNginx(ProxyRequest request)
        => await this.SendAsync(this.nginxClient, request, request.WebhookUrl);

    [HttpPost(Name = "Send/Yarp")]
    public async Task<IActionResult> SendYarp(ProxyRequest request)
        => await this.SendAsync(this.yarpClient, request);

    private async Task<IActionResult> SendAsync(HttpClient client, ProxyRequest request, string? url = null)
    {
        using var message = CreateMessage(request);

        if (!string.IsNullOrWhiteSpace(url))
        {
            message.RequestUri = new Uri(url);
        }

        var result = await client.SendAsync(message);

        result.EnsureSuccessStatusCode();

        return Ok();
    }

    private static HttpRequestMessage CreateMessage(ProxyRequest request)
    {
        var message = new HttpRequestMessage();

        message.Content = JsonContent.Create(request);

        //Header only for YARP
        message.Headers.Add(WEBHOOK_URL_HEADER, request.WebhookUrl);

        message.Headers.Add("id", Guid.NewGuid().ToString());
        message.Headers.Add("version", "2023.09");

        message.Method = HttpMethod.Post;

        return message;
    }
}
