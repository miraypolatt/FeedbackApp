using FeedbackApp.Application.Interfaces;
using FeedbackApp.Domain.Entities;
using FeedbackApp.Domain.Events;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace FeedbackApp.Infrastructure.Messaging
{
    public class RabbitMqPublisherService : IMessageQueuePublisher
    {
        private readonly IConfiguration _configuration;
        private readonly ConnectionFactory _factory;
        private readonly string _queueName;

        public RabbitMqPublisherService(IConfiguration configuration)
        {
            _configuration = configuration;

            _factory = new ConnectionFactory
            {
                HostName = _configuration["RabbitMQ:Host"] ?? "localhost",
                UserName = _configuration["RabbitMQ:Username"] ?? "guest",
                Password = _configuration["RabbitMQ:Password"] ?? "guest"
            };

            _queueName = _configuration["RabbitMQ:QueueName"] ?? "feedback_queue";
        }
        public Task PublishAsync<T>(T message)
        {
            using var connection = _factory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.QueueDeclare(
                queue: _queueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            var serializedMessage = JsonSerializer.Serialize(message);
            Console.WriteLine("Serialized message to be sent: " + serializedMessage);
            var body = Encoding.UTF8.GetBytes(serializedMessage);


            var properties = channel.CreateBasicProperties();
            properties.Persistent = true;

            channel.BasicPublish(
                exchange: "",
                routingKey: _queueName,
                basicProperties: properties,
                body: body);

            return Task.CompletedTask;
        }

    }
}