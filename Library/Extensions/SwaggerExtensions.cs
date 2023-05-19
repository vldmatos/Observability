using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace Library.Extensions;

public static class SwaggerExtensions
{
    public static void AddSwagger(this IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo { Title = "Observability", Version = "v1" });
            options.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
        });
    }

    public static void UseSwaggerDoc(this IApplicationBuilder applicationBuilder)
    {
        applicationBuilder.UseSwagger();
        applicationBuilder.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "Observability");
            options.RoutePrefix = "swagger";
        });
    }
}