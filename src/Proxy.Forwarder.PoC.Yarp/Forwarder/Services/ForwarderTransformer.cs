namespace Proxy.Forwarder.PoC.Yarp.Forwarder.Services;

using global::Yarp.ReverseProxy.Forwarder;

internal sealed class ForwarderTransformer : HttpTransformer
{
    public override async ValueTask TransformRequestAsync(HttpContext httpContext, HttpRequestMessage proxyRequest, string destinationPrefix, CancellationToken cancellationToken)
    {
        // Copy all request headers
        await base.TransformRequestAsync(httpContext, proxyRequest, destinationPrefix, cancellationToken);

        // Suppress the original request header, use the one from the destination Uri.
        proxyRequest.Headers.Host = null;
        proxyRequest.Headers.Remove(Constants.WEBHOOK_URL_HEADER);
    }
}
