using FCG.Game.Application.Services.Interfaces;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using System.Text;

namespace FCG.Game.Infrastructure.Messaging
{
    public class RabbitMqSettings
    {
        public string HostName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }

    public class RabbitMqMessagePublisher : IMessagePublisher
    {
        private readonly RabbitMqSettings _settings;

        public RabbitMqMessagePublisher(IOptions<RabbitMqSettings> settings)
        {
            _settings = settings.Value;
        }

        public void Publish(string message, string queueName)
        {
            var factory = new ConnectionFactory()
            {
                HostName = _settings.HostName,
                UserName = _settings.UserName,
                Password = _settings.Password
            };

            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: queueName,
                                     durable: true,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                var body = Encoding.UTF8.GetBytes(message);

                var properties = channel.CreateBasicProperties();
                properties.Persistent = true;

                channel.BasicPublish(exchange: "",
                                     routingKey: queueName,
                                     basicProperties: properties,
                                     body: body);
            }
        }
    }
}
