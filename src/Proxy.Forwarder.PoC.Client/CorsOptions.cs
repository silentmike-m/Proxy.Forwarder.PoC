namespace Proxy.Forwarder.PoC.Client;

internal sealed record CorsOptions
{
    public static readonly string SECTION_NAME = "Cors";

    public string[] AllowedHeaders { get; init; } = Array.Empty<string>();
    public string[] AllowedMethods { get; init; } = Array.Empty<string>();
    public string[] AllowedOrigins { get; init; } = Array.Empty<string>();
}
