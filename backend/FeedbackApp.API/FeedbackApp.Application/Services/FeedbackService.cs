using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FeedbackApp.Application.DTOs;
using FeedbackApp.Application.Helpers;
using FeedbackApp.Application.Interfaces;
using FeedbackApp.Domain.Entities;
using FeedbackApp.Domain.Events;
using Microsoft.Extensions.Logging;

namespace FeedbackApp.Application.Services
{

    public class FeedbackService : IFeedbackService
    {
        private readonly IFeedbackRepository _repository;
        private readonly IMessageQueuePublisher _publisher;
        private readonly ILogger<FeedbackService> _logger;
        private readonly IErrorLogger _errorLogger;

        public FeedbackService(
            IFeedbackRepository repository,
            IMessageQueuePublisher publisher,
            ILogger<FeedbackService> logger,
            IErrorLogger errorLogger)
        {
            _repository = repository;
            _publisher = publisher;
            _logger = logger;
            _errorLogger = errorLogger;
        }

        public async Task<IEnumerable<Feedback>> GetAllAsync()
        {
            try
            {
                _logger.LogInformation("Tüm geri bildirimler getiriliyor.");
                return await _repository.GetAllAsync();
            }
            catch (Exception ex)
            {
                var error = ErrorLogFactory.FromException(ex, nameof(GetAllAsync));
                await _errorLogger.LogAsync(error);
                _logger.LogError(ex, "GetAllAsync sırasında hata oluştu.");
                throw;
            }
        }

        public async Task<Feedback?> GetByIdAsync(string id)
        {
            try
            {
                _logger.LogInformation($"ID ile geri bildirim getiriliyor: {id}");
                return await _repository.GetByIdAsync(id);
            }
            catch (Exception ex)
            {
                var error = ErrorLogFactory.FromException(ex, nameof(GetByIdAsync));
                await _errorLogger.LogAsync(error);
                _logger.LogError(ex, "GetByIdAsync sırasında hata oluştu.");
                throw;
            }
        }

        public async Task CreateAsync(Feedback feedback)
        {
            try
            {
                feedback.Id = Guid.NewGuid();
                feedback.SubmittedAt = DateTime.UtcNow;

                var evt = new FeedbackEvent
                {
                    EventType = FeedbackEventType.Created,
                    Feedback = feedback
                };

                _logger.LogInformation("Feedback 'Created' event'i RabbitMQ kuyruğuna gönderiliyor.");
                await _publisher.PublishAsync(evt);
            }
            catch (Exception ex)
            {
                var error = ErrorLogFactory.FromException(ex, nameof(CreateAsync));
                await _errorLogger.LogAsync(error);
                _logger.LogError(ex, "Feedback oluşturulurken hata oluştu.");
                throw;
            }
        }

        public async Task<bool> UpdateAsync(string id, Feedback feedback)
        {
            try
            {
                _logger.LogInformation($"Update eventi kuyruğa gönderiliyor: {id}");
                await _publisher.PublishAsync(new FeedbackEvent
                {
                    EventType = FeedbackEventType.Updated,
                    Feedback = feedback
                });
                return true;
            }
            catch (Exception ex)
            {
                var error = ErrorLogFactory.FromException(ex, nameof(UpdateAsync));
                await _errorLogger.LogAsync(error);
                _logger.LogError(ex, "UpdateAsync sırasında hata oluştu.");
                throw;
            }
        }


        public async Task<bool> DeleteAsync(string id)
        {
            try
            {
                _logger.LogInformation($"Delete eventi kuyruğa gönderiliyor: {id}");
                await _publisher.PublishAsync(new FeedbackEvent
                {
                    EventType = FeedbackEventType.Deleted,
                    Feedback = new Feedback { Id = Guid.Parse(id) }
                });
                return true;
            }
            catch (Exception ex)
            {
                var error = ErrorLogFactory.FromException(ex, nameof(DeleteAsync));
                await _errorLogger.LogAsync(error);
                _logger.LogError(ex, "DeleteAsync sırasında hata oluştu.");
                throw;
            }
        }

    }
}
