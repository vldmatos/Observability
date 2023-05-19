using Elastic.Apm.SerilogEnricher;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Exceptions;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;

namespace Library.Extensions;

public static class SerilogExtensions
{
    public static WebApplicationBuilder AddSerilog(this WebApplicationBuilder builder, IConfiguration configuration, string applicationName)
    {
        Log.Logger = CreateLog(configuration, applicationName);

        builder.Logging.ClearProviders();
        builder.Host.UseSerilog(Log.Logger, true);

        return builder;
    }

    public static IServiceCollection AddSerilog(this IServiceCollection services, IConfiguration configuration, string applicationName)
    {
        Log.Logger = CreateLog(configuration, applicationName);

        services.AddSingleton(Log.Logger);
        services.AddLogging(loggingBuilder =>
        {
            loggingBuilder.ClearProviders();
            loggingBuilder.AddSerilog(Log.Logger, true);
        });

        return services;
    }

    private static Serilog.ILogger CreateLog(IConfiguration configuration, string applicationName)
    {
        var loggerConfiguration = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .Enrich.FromLogContext()
            .Enrich.WithMachineName()
            .Enrich.WithEnvironmentUserName()
            .Enrich.WithExceptionDetails()
            .Enrich.WithProperty("ApplicationName", $"{applicationName} - {configuration.GetSection("DOTNET_ENVIRONMENT")?.Value}")
            .Enrich.WithElasticApmCorrelationInfo()
            .WriteTo.Console(outputTemplate: "[{ElasticApmTraceId} {ElasticApmTransactionId} {Message:lj} {NewLine}{Exception}")
            .CreateLogger();

        return loggerConfiguration;
    }
}