using Microsoft.AspNetCore.HttpLogging;
using Proxy.Forwarder.PoC.Client;
using Proxy.Forwarder.PoC.Client.HealthChecks;
using Proxy.Forwarder.PoC.Client.Nginx;
using Proxy.Forwarder.PoC.Client.Yarp;
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

builder.Services.AddNginx(builder.Configuration);
builder.Services.AddYarp(builder.Configuration);

builder.Services.AddHealthChecks(builder.Configuration);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

    app.UseSwagger();
    app.UseSwaggerUI();

    app.UseHealthChecks();

    app.MapControllers();

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
