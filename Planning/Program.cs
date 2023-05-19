using Library.Data;
using Library.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Planning
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.AddSerilog(builder.Configuration, "Planning");
            builder.Services.AddHangFire(builder.Configuration);
            builder.Services.AddElasticsearch(builder.Configuration);

            var options = new DbContextOptionsBuilder<ObservabilityContext>();
            options.UseSqlServer(builder.Configuration.GetConnectionString("ObservabilityContextConnection"));
            builder.Services.AddScoped(scope => new ObservabilityContext(options.Options));

            var application = builder.Build();
            application.UseHangFire();
            application.UseElasticApm(builder.Configuration);
            application.ExecuteJobs(builder.Configuration);

            application.Run();
        }
    }
}