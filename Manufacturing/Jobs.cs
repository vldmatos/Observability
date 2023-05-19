using Elastic.Apm;
using Elastic.Apm.Api;
using Hangfire;
using Message = Library.Models.Message;
using Product = Library.Models.Product;

namespace Manufacturing
{
    public static class Jobs
    {
        private static readonly Random _random = new();
        private static HttpClient _httpClient = null!;
        private static Library.Services.RabbitMQ _rabbitMQ = null!;

        public static WebApplication ExecuteJobs(this WebApplication application, IConfiguration configuration)
        {
            _rabbitMQ = new(configuration);
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(configuration["API_Address"])
            };

            RecurringJob.AddOrUpdate("manufacturing-send-entries", () => SendEntries(), "*/20 * * * * *");
            RecurringJob.AddOrUpdate("manufacturing-request-finalize", () => RequestFinalize(), "*/30 * * * * *");
            RecurringJob.AddOrUpdate("manufacturing-request-supply", () => RequestSupply(), "*/40 * * * * *");
            RecurringJob.AddOrUpdate("manufacturing-list-products", () => ListProducts(), "*/45 * * * * *");

            return application;
        }

        public static void SendEntries()
        {
            Agent.Tracer
                 .CaptureTransaction(nameof(SendEntries), ApiConstants.TypeMessaging, () =>
                 {
                     var message = new Message()
                     {
                         Date = DateTime.Now,
                         Code = Message.EntryCode,
                         Products = Product.CreateList().ToList()
                     };

                     _rabbitMQ.Send(message);
                 });
        }

        public static void RequestFinalize()
        {
            Agent.Tracer
                 .CaptureTransaction(nameof(RequestFinalize), ApiConstants.TypeRequest, () =>
                 {
                     var product = new Product();

                     _httpClient.PostAsJsonAsync("finalize", product)
                                 .GetAwaiter()
                                 .GetResult();
                 });
        }

        public static void RequestSupply()
        {
            Agent.Tracer
                 .CaptureTransaction(nameof(RequestSupply), ApiConstants.TypeRequest, () =>
                 {
                     var index = _random.Next(0, Product.Workstations.Count - 1);
                     var workstation = Product.Workstations[index];

                     _httpClient.PostAsJsonAsync("supply", new List<string> { workstation })
                                .GetAwaiter()
                                .GetResult();
                 });
        }

        public static void ListProducts()
        {
            Agent.Tracer
                 .CaptureTransaction(nameof(ListProducts), ApiConstants.TypeRequest, () =>
                 {
                     _ = _httpClient.GetAsync("products")
                                    .GetAwaiter()
                                    .GetResult();
                 });
        }
    }
}