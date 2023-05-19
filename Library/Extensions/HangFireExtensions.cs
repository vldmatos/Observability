using Hangfire;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Library.Extensions
{
    public static class HangFireExtensions
    {
        public static void AddHangFire(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHangfire(options => options
                    .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                    .UseSimpleAssemblyNameTypeSerializer()
                    .UseRecommendedSerializerSettings()
                    .UseSqlServerStorage(configuration.GetConnectionString("HangfireContextConnection")));

            services.AddHangfireServer();
        }

        public static void UseHangFire(this IApplicationBuilder application)
        {
            application.UseRouting();
            application.UseEndpoints(endpoints =>
            {
                endpoints.MapHangfireDashboard();
            });
        }
    }
}