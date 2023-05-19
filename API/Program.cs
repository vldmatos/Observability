using Library.Data;
using Library.Extensions;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.AddSerilog(builder.Configuration, "API");
            builder.Services.AddApiConfiguration();
            builder.Services.AddElasticsearch(builder.Configuration);
            builder.Services.AddSwagger();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddDbContext<ObservabilityContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("ObservabilityContextConnection")));

            var application = builder.Build();
            application.UseApiConfiguration(application.Environment);
            application.UseSwaggerDoc();
            application.UseElasticApm(builder.Configuration);
            application.UseOperations();

            application.Run();
        }
    }
}