using Library.Models;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace Library.Services
{
    public class RabbitMQ
    {
        private readonly ConnectionFactory _connectionFactory;
        private readonly Models.Configurations.RabbitMQ _rabbitMQConfiguration;
        private readonly IBasicProperties _properties;
        public readonly IModel Channel;
        public string QueueName => _rabbitMQConfiguration.QueueName;

        public RabbitMQ(IConfiguration configuration)
        {
            _rabbitMQConfiguration = configuration.GetSection(nameof(Models.Configurations.RabbitMQ))
                                     .Get<Models.Configurations.RabbitMQ>();

            _connectionFactory = new ConnectionFactory()
            {
                HostName = _rabbitMQConfiguration.HostName,
                Port = _rabbitMQConfiguration.Port,
                UserName = _rabbitMQConfiguration.UserName,
                Password = _rabbitMQConfiguration.Password
            };

            Channel = _connectionFactory.CreateConnection()
                                         .CreateModel();

            Channel.QueueDeclareNoWait(_rabbitMQConfiguration.QueueName, true, false, false, null);
            _properties = Channel.CreateBasicProperties();
            _properties.Persistent = true;
        }

        public void Send(Message model)
        {
            Channel.BasicPublish(exchange: string.Empty,
                                  routingKey: _rabbitMQConfiguration.QueueName,
                                  basicProperties: _properties,
                                  body: Encoding.UTF8.GetBytes(JsonSerializer.Serialize(model)));
        }

        public Message Receive()
        {
            Message message = null;

            var eventingConsumer = new EventingBasicConsumer(Channel);
            eventingConsumer.Received += (model, content) =>
            {
                var body = Encoding.UTF8.GetString(content.Body.ToArray());
                Channel.BasicAck(deliveryTag: content.DeliveryTag, multiple: false);

                message = JsonSerializer.Deserialize<Message>(body);
            };

            Channel.BasicConsume(queue: _rabbitMQConfiguration.QueueName, autoAck: false, consumer: eventingConsumer);

            return message;
        }
    }
}