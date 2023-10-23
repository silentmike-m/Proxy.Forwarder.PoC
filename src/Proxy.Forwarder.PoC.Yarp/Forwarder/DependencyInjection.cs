namespace Proxy.Forwarder.PoC.Yarp.Forwarder;

using Proxy.Forwarder.PoC.Yarp.Forwarder.Services;

internal static class DependencyInjection
{
    public static IServiceCollection AddForwarder(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<ForwarderOptions>(configuration.GetSection(ForwarderOptions.SECTION_NAME));

        services.AddHttpForwarder();

        services.AddScoped<ForwarderTransformer>();
        services.AddScoped<ForwarderService>();

        return services;
    }
}
