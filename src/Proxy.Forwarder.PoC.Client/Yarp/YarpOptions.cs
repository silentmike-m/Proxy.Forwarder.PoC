namespace Proxy.Forwarder.PoC.Client.Yarp;

internal sealed record YarpOptions
{
    public static readonly string HTTP_CLIENT_NAME = "YarpClient";
    public static readonly string SECTION_NAME = "Yarp";

    public Uri BaseAddress { get; init; } = new("about:blank");
    public Uri HealthCheckAddress { get; init; } = new("about:blank");
}
