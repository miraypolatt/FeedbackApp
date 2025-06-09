using FeedbackApp.Application.Helpers;
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
    public class FeedbackProcessor : IFeedbackProcessor
    {
        private readonly IFeedbackRepository _feedbackRepository;
        private readonly IErrorLogger _errorLogger;

        public FeedbackProcessor(IFeedbackRepository feedbackRepository, IErrorLogger errorLogger)
        {
            _feedbackRepository = feedbackRepository;
            _errorLogger = errorLogger;
        }

        public async Task ProcessAsync(string message)
        {
            try
            {
                Console.WriteLine("Processing message: " + message);

                var eventMessage = JsonSerializer.Deserialize<FeedbackEvent>(message, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (eventMessage?.Feedback != null)
                {
                    await _feedbackRepository.CreateAsync(eventMessage.Feedback);
                }
                else
                {
                    await _errorLogger.LogAsync(ErrorLogFactory.FromException(
                        new Exception("Deserialized eventMessage or Feedback is null"),
                        nameof(FeedbackProcessor),
                        requestPath: "RabbitMQ message: " + message));
                }
            }
            catch (Exception ex)
            {
                await _errorLogger.LogAsync(ErrorLogFactory.FromException(
                    ex,
                    nameof(FeedbackProcessor),
                    requestPath: "RabbitMQ message: " + message));
            }
        }
    }

}