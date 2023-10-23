namespace Proxy.Forwarder.PoC.Client.Nginx;

internal sealed record NginxOptions
{
    public static readonly string HTTP_CLIENT_NAME = "NginxClient";
    public static readonly string SECTION_NAME = "Nginx";

    public Uri BaseAddress { get; init; } = new("about:blank");
    public Uri HealthCheckAddress { get; init; } = new("about:blank");
}
