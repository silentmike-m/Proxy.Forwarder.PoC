namespace Proxy.Forwarder.PoC.Client.HealthChecks.Models;

internal sealed record ComponentHealthCheck
{
    public string Component { get; init; } = string.Empty;
    public string? Description { get; init; } = default;
    public string? ErrorMessage { get; init; } = default;
    public int HealthCheckDurationInMilliseconds { get; init; } = default;
    public string Status { get; init; } = string.Empty;
}
