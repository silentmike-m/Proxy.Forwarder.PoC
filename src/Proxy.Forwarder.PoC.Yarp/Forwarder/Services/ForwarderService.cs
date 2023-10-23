namespace Proxy.Forwarder.PoC.Yarp.Forwarder.Services;

using System.Diagnostics;
using System.Net;
using System.Net.Security;
using global::Yarp.ReverseProxy.Forwarder;
using Microsoft.Extensions.Options;

internal sealed class ForwarderService
{
    private readonly IHttpForwarder forwarder;
    private readonly ForwarderOptions forwarderOptions;
    private readonly ILogger<ForwarderService> logger;
    private readonly ForwarderTransformer transformer;

    public ForwarderService(IHttpForwarder forwarder, IOptions<ForwarderOptions> forwarderOptions, ILogger<ForwarderService> logger, ForwarderTransformer transformer)
    {
        this.forwarder = forwarder;
        this.forwarderOptions = forwarderOptions.Value;
        this.logger = logger;
        this.transformer = transformer;
    }

    public async Task<IResult> Invoke(HttpContext context)
    {
        this.logger.LogInformation("Try to forward request");

        if (!context.Request.Headers.TryGetValue(Constants.WEBHOOK_URL_HEADER, out var webhookUrlValues))
        {
            this.logger.LogDebug("Request doesn't contain webhook url header");

            return Results.Ok();
        }

        var webhookUrl = webhookUrlValues.FirstOrDefault();

        if (string.IsNullOrEmpty(webhookUrl))
        {
            this.logger.LogDebug("Request webhook url is empty");

            return Results.Ok();
        }

        //https://microsoft.github.io/reverse-proxy/articles/direct-forwarding.html#the-http-client
        //Always use HttpMessageInvoker rather than HttpClient, HttpClient buffers responses by default.
        //Buffering breaks streaming scenarios and increases memory usage and latency.

        try
        {
            using var socketHandler = this.GetSocketsHttpHandler();
            using var httpClient = new HttpMessageInvoker(socketHandler);

            var requestOptions = new ForwarderRequestConfig { ActivityTimeout = TimeSpan.FromSeconds(this.forwarderOptions.ActivityTimeoutInSeconds) };

            var error = await this.forwarder.SendAsync(context, webhookUrl, httpClient, requestOptions, this.transformer);

            if (error != ForwarderError.None)
            {
                var errorFeature = context.Features.Get<IForwarderErrorFeature>();

                return Results.Problem(errorFeature?.Exception?.Message ?? $"Unknown proxy exception: {error}");
            }
        }
        catch (Exception exception)
        {
            return Results.Problem(exception.Message);
        }

        return Results.Ok();
    }

    private SocketsHttpHandler GetSocketsHttpHandler()
    {
        var socketHandler = new SocketsHttpHandler
        {
            UseProxy = false,
            AllowAutoRedirect = false,
            AutomaticDecompression = DecompressionMethods.None,
            UseCookies = false,
            ActivityHeadersPropagator = new ReverseProxyPropagator(DistributedContextPropagator.Current),
            ConnectTimeout = TimeSpan.FromSeconds(this.forwarderOptions.ConnectTimeoutInSeconds),
        };

#if DEBUG
        socketHandler.SslOptions = new SslClientAuthenticationOptions
        {
            RemoteCertificateValidationCallback = (_, _, _, _) => true,
        };
#endif

        return socketHandler;
    }
}
