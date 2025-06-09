using FeedbackApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FeedbackApp.Domain.Events
{
    public class FeedbackEvent
    {
        public FeedbackEventType EventType { get; set; }
        public Feedback Feedback { get; set; } = default!;
    }

    public enum FeedbackEventType
    {
        Created,
        Updated,
        Deleted
    }
}
