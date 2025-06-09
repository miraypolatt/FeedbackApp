using FeedbackApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FeedbackApp.Application.Interfaces
{
    public interface IFeedbackProcessor
    {
        Task ProcessAsync(string message);
    }
}
