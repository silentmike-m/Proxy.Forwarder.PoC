namespace Proxy.Forwarder.PoC.Client.HealthChecks;

using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Proxy.Forwarder.PoC.Client.HealthChecks.Models;
using Proxy.Forwarder.PoC.Client.Nginx;
using Proxy.Forwarder.PoC.Client.Yarp;

internal static class DependencyInjection
{
    private const string NGINX_HEALTH_CHECK_NAME = "Nginx";
    private const string YARP_HEALTH_CHECK_NAME = "Yarp";

    public static void AddHealthChecks(this IServiceCollection services, IConfiguration configuration)
    {
        var nginxOptions = configuration.GetSection(NginxOptions.SECTION_NAME).Get<NginxOptions>();
        nginxOptions ??= new NginxOptions();

        var yarpOptions = configuration.GetSection(YarpOptions.SECTION_NAME).Get<YarpOptions>();
        yarpOptions ??= new YarpOptions();

        services.AddHealthChecks()
            .AddUrlGroup(nginxOptions.HealthCheckAddress, NGINX_HEALTH_CHECK_NAME)
            .AddUrlGroup(yarpOptions.HealthCheckAddress, YARP_HEALTH_CHECK_NAME)
            ;
    }

    public static void UseHealthChecks(this IApplicationBuilder app)
    {
        var healthOptions = new HealthCheckOptions
        {
            ResponseWriter = HealthCheckResponseWriterAsync,
            ResultStatusCodes =
            {
                [HealthStatus.Healthy] = StatusCodes.Status200OK,
                [HealthStatus.Degraded] = StatusCodes.Status200OK,
                [HealthStatus.Unhealthy] = StatusCodes.Status200OK,
            },
        };

        app.UseHealthChecks("/hc", healthOptions);
    }

    private static async Task HealthCheckResponseWriterAsync(HttpContext context, HealthReport report)
    {
        context.Response.ContentType = "application/json";

        var componentHealthChecks = new List<ComponentHealthCheck>();

        foreach (var (component, componentReport) in report.Entries)
        {
            var componentHealthCheck = new ComponentHealthCheck
            {
                Component = component,
                Description = componentReport.Description,
                ErrorMessage = componentReport.Exception?.Message,
                HealthCheckDurationInMilliseconds = componentReport.Duration.Milliseconds,
                Status = componentReport.Status.ToString(),
            };

            componentHealthChecks.Add(componentHealthCheck);
        }

        var healthCheck = new HealthCheck
        {
            HealthChecks = componentHealthChecks,
            HealthCheckDurationInMilliseconds = report.TotalDuration.Milliseconds,
            Status = report.Status.ToString(),
        };

        var jsonOptions = new JsonSerializerOptions
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            WriteIndented = true,
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(healthCheck, jsonOptions));
    }
}
