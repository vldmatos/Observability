using Library.Extensions;

namespace Manufacturing
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.AddSerilog(builder.Configuration, "Manufacturing");
            builder.Services.AddHangFire(builder.Configuration);
            builder.Services.AddElasticsearch(builder.Configuration);

            var application = builder.Build();
            application.UseHangFire();
            application.UseElasticApm(builder.Configuration);
            application.ExecuteJobs(builder.Configuration);

            application.Run();
        }
    }
}