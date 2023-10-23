using Microsoft.AspNetCore.HttpLogging;
using Proxy.Forwarder.PoC.Yarp;
using Proxy.Forwarder.PoC.Yarp.Forwarder;
using Proxy.Forwarder.PoC.Yarp.Forwarder.Services;
using Serilog;

const int EXIT_FAILURE = 1;
const int EXIT_SUCCESS = 0;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables("CONFIG_");

builder.Host.UseSerilog(
    (_, loggerConfiguration) =>
    {
        loggerConfiguration.ReadFrom.Configuration(builder.Configuration);
    }
);

builder.Services.AddHttpLogging(options => options.LoggingFields = HttpLoggingFields.All);

builder.Services.AddHealthChecks();

builder.Services.AddForwarder(builder.Configuration);

var corsOptions = builder.Configuration.GetSection(CorsOptions.SECTION_NAME).Get<CorsOptions>();
corsOptions ??= new CorsOptions();

builder.Services
    .AddCors(
        options => options.AddPolicy(
            "Default", corsBuilder => corsBuilder
                .WithOrigins(corsOptions.AllowedOrigins)
                .AllowCredentials()
                .WithHeaders(corsOptions.AllowedHeaders)
                .WithMethods(corsOptions.AllowedMethods)
        )
    );

try
{
    Log.Information("Starting host...");

    var app = builder.Build();

    app.UseCors("Default");

    app.UseHealthChecks("/hc");

    app.UseHttpsRedirection();

    app.UseHttpLogging();

    app.UseRouting();

    app.UseEndpoints(endpoints =>
        {
            endpoints.MapPost("{**catch-all}", async (HttpContext context, ForwarderService forwarder) => await forwarder.Invoke(context));
        }
    );

    await app.RunAsync();

    return EXIT_SUCCESS;
}
catch (Exception exception)
{
    Log.Fatal(exception, "Host terminated unexpectedly");

    return EXIT_FAILURE;
}
finally
{
    await Log.CloseAndFlushAsync();
}
