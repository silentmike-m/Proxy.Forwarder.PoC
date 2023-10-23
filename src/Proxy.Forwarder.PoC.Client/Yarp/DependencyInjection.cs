namespace Proxy.Forwarder.PoC.Client.Yarp;

internal static class DependencyInjection
{
    public static IServiceCollection AddYarp(this IServiceCollection services, IConfiguration configuration)
    {
        var options = configuration.GetSection(YarpOptions.SECTION_NAME).Get<YarpOptions>();
        options ??= new YarpOptions();

        services.AddHttpClient(YarpOptions.HTTP_CLIENT_NAME, client => client.BaseAddress = options.BaseAddress);

        return services;
    }
}
