using Elastic.Apm;
using Elastic.Apm.Api;
using Hangfire;
using Library.Data;
using Library.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using Message = Library.Models.Message;

namespace Planning
{
    public static class Jobs
    {
        private static readonly Random _random = new();
        private static Library.Services.RabbitMQ _rabbitMQ = null!;
        private static HttpClient _httpClient = null!;
        private static IServiceScope _serviceScope = null!;

        public static WebApplication ExecuteJobs(this WebApplication application, IConfiguration configuration)
        {
            _rabbitMQ = new(configuration);
            _serviceScope = application.Services.CreateScope();

            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(configuration["API_Address"])
            };

            RecurringJob.AddOrUpdate("planning-get-messages", () => GetMessages(), "*/20 * * * * *");
            RecurringJob.AddOrUpdate("planning-request-error", () => RequestError(), "*/55 * * * * *");

            return application;
        }

        public static void RequestError()
        {
            Agent.Tracer
                 .CaptureTransaction(nameof(RequestError), ApiConstants.TypeRequest, () =>
                 {
                     _ = _httpClient.GetAsync("error")
                                    .GetAwaiter()
                                    .GetResult();
                 });
        }

        public static void GetMessages()
        {
            Agent.Tracer
                 .CaptureTransaction(nameof(GetMessages), ApiConstants.TypeMessaging, () =>
                 {
                     var eventingConsumer = new EventingBasicConsumer(_rabbitMQ.Channel);
                     eventingConsumer.Received += (model, content) =>
                     {
                         var body = Encoding.UTF8.GetString(content.Body.ToArray());
                         _rabbitMQ.Channel.BasicAck(deliveryTag: content.DeliveryTag, multiple: false);

                         var message = JsonSerializer.Deserialize<Message>(body);
                         if (message is null)
                             return;

                         Save(Process(message));
                         SupplyAllWorkStations();
                     };

                     _rabbitMQ.Channel.BasicConsume(queue: _rabbitMQ.QueueName, autoAck: false, consumer: eventingConsumer);
                 });
        }

        private static List<Product> Process(Message message)
        {
            List<Product> sequencialProducts = new();

            if (message is null)
                return sequencialProducts;

            int index = 1;
            foreach (var product in message.Products)
            {
                product.Name = $"{product.Name}-{index}";
                product.Status = "sequenced";
                index++;
            }

            return message.Products;
        }

        private static void Save(List<Product> products)
        {
            if (products is null || products.Count == 0)
                return;

            var observabilityContext = _serviceScope.ServiceProvider.GetRequiredService<ObservabilityContext>();

            foreach (var product in products)
                observabilityContext.Products.Add(product);

            observabilityContext.SaveChanges();
        }

        private static void SupplyAllWorkStations()
        {
            _httpClient.PostAsJsonAsync("supply", Product.Workstations)
                       .GetAwaiter()
                       .GetResult();
        }
    }
}