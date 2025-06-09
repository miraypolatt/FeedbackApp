using FeedbackApp.Domain.Entities;
using FeedbackApp.Domain.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FeedbackApp.Application.Interfaces
{
        public interface IMessageQueuePublisher
        {
            Task PublishAsync<T>(T message);
        }
}
