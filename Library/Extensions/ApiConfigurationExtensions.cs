using Library.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Library.Extensions;

public static class ApiConfigurationExtensions
{
    public static void AddApiConfiguration(this IServiceCollection services)
    {
        services.AddRouting(options => options.LowercaseUrls = true);

        services.AddHttpClient();
        services.AddControllers();
    }

    public static void UseApiConfiguration(this IApplicationBuilder application, IWebHostEnvironment environment)
    {
        if (environment.IsDevelopment())
        {
            application.UseDeveloperExceptionPage();
        }

        application.UseMiddleware<RequestSerilLogMiddleware>();
        application.UseMiddleware<ErrorHandlingMiddleware>();

        application.UseHttpsRedirection();
        application.UseRouting();
    }
}