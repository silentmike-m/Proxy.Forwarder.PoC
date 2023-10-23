namespace Proxy.Forwarder.PoC.Client.HealthChecks.Models;

internal sealed record HealthCheck
{
    public int HealthCheckDurationInMilliseconds { get; init; } = default;
    public IReadOnlyList<ComponentHealthCheck> HealthChecks { get; init; } = new List<ComponentHealthCheck>().AsReadOnly();
    public string Status { get; init; } = string.Empty;
}
