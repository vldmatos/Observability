using Elastic.Apm.NetCoreAll;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nest;

namespace Library.Extensions;

public static class ElasticsearchExtensions
{
    public static void AddElasticsearch(this IServiceCollection services, IConfiguration configuration)
    {
        var settings = new ConnectionSettings(new Uri(configuration["ElasticsearchSettings:uri"]));

        var defaultIndex = configuration["ElasticsearchSettings:defaultIndex"];

        if (!string.IsNullOrEmpty(defaultIndex))
            settings = settings.DefaultIndex(defaultIndex);

        var basicAuthUser = configuration["ElasticsearchSettings:username"];
        var basicAuthPassword = configuration["ElasticsearchSettings:password"];

        if (!string.IsNullOrEmpty(basicAuthUser) && !string.IsNullOrEmpty(basicAuthPassword))
            settings = settings.BasicAuthentication(basicAuthUser, basicAuthPassword);

        var client = new ElasticClient(settings);

        services.AddSingleton<IElasticClient>(client);

        services.Configure<KestrelServerOptions>(options => options.AllowSynchronousIO = true);
        services.Configure<IISServerOptions>(options => options.AllowSynchronousIO = true);
    }

    public static void UseElasticApm(this IApplicationBuilder application, IConfiguration configuration)
    {
        application.UseAllElasticApm(configuration);
    }
}