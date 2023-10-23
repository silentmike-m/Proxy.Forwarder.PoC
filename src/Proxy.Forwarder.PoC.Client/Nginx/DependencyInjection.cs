namespace Proxy.Forwarder.PoC.Client.Nginx;

using System.Net;

internal static class DependencyInjection
{
    public static IServiceCollection AddNginx(this IServiceCollection services, IConfiguration configuration)
    {
        var options = configuration.GetSection(NginxOptions.SECTION_NAME).Get<NginxOptions>();
        options ??= new NginxOptions();

        services
            .AddHttpClient(NginxOptions.HTTP_CLIENT_NAME)
            .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
            {
                AllowAutoRedirect = true,
                Proxy = new WebProxy
                {
                    Address = options.BaseAddress,
                    BypassProxyOnLocal = false,
                    UseDefaultCredentials = true,
                },
                UseProxy = true,
            });

        return services;
    }
}
