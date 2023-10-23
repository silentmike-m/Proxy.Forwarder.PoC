namespace Proxy.Forwarder.PoC.Yarp.Forwarder;

internal sealed record ForwarderOptions
{
    public static readonly string SECTION_NAME = "Forwarder";

    public int ActivityTimeoutInSeconds { get; init; } = 100;
    public int ConnectTimeoutInSeconds { get; init; } = 15;
}
