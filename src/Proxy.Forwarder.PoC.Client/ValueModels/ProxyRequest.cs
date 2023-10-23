namespace Proxy.Forwarder.PoC.Client.ValueModels;

using System.Text.Json.Serialization;

public sealed record ProxyRequest
{
    [JsonPropertyName("id")] public Guid Id { get; init; } = Guid.Empty;
    [JsonPropertyName("name")] public string Name { get; init; } = string.Empty;
    [JsonPropertyName("webhook_url")] public string WebhookUrl { get; init; } = string.Empty;
}
