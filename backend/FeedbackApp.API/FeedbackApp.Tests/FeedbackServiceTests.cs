using System;
using System.Threading.Tasks;
using FeedbackApp.Application.Interfaces;
using FeedbackApp.Application.Services;
using FeedbackApp.Domain.Entities;
using FeedbackApp.Domain.Events;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace FeedbackApp.Tests.Application.Services
{
    public class FeedbackServiceTests
    {
        private readonly Mock<IFeedbackRepository> _repoMock;
        private readonly Mock<IMessageQueuePublisher> _publisherMock;
        private readonly Mock<IErrorLogger> _errorLoggerMock;
        private readonly Mock<ILogger<FeedbackService>> _loggerMock;
        private readonly FeedbackService _service;

        public FeedbackServiceTests()
        {
            _repoMock = new Mock<IFeedbackRepository>();
            _publisherMock = new Mock<IMessageQueuePublisher>();
            _errorLoggerMock = new Mock<IErrorLogger>();
            _loggerMock = new Mock<ILogger<FeedbackService>>();

            _service = new FeedbackService(
                _repoMock.Object,
                _publisherMock.Object,
                _loggerMock.Object,
                _errorLoggerMock.Object);
        }

        [Fact]
        public async Task CreateAsync_Should_Publish_Feedback_Creation_Event()
        {
            var feedback = new Feedback
            {
                Name = "Test",
                Email = "test@example.com",
                Message = "hello"
            };

            await _service.CreateAsync(feedback);

            _publisherMock.Verify(p => p.PublishAsync(
                It.Is<FeedbackEvent>(e =>
                    e.EventType == FeedbackEventType.Created &&
                    e.Feedback.Name == "Test")), Times.Once);
        }

        [Fact]
        public async Task GetAllAsync_Should_Return_Repository_Data()
        {
            IEnumerable<Feedback> feedbacks = new List<Feedback>
           {
               new Feedback { Name = "Test1", Email = "1@test.com" },
               new Feedback { Name = "Test2", Email = "2@test.com" }
           };
            _repoMock.Setup(r => r.GetAllAsync()).Returns(Task.FromResult(feedbacks.ToList()));

            var result = await _service.GetAllAsync();

            Assert.Equal(2, result?.Count());
        }
    }
}
