using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FeedbackApp.Domain.Entities
{
    public class ErrorLog
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Source { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string? StackTrace { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public string? RequestPath { get; set; }
        public string? UserId { get; set; }
        public string? CorrelationId { get; set; }
        public string? ExceptionType { get; set; }
        public Dictionary<string, string>? AdditionalData { get; set; }
    }
}
