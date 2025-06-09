using FeedbackApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FeedbackApp.Application.Helpers
{
    public static class ErrorLogFactory
    {
        public static ErrorLog FromException(Exception ex, string source, string? userId = null, string? correlationId = null, string? requestPath = null)
        {
            return new ErrorLog
            {
                Source = source,
                Message = ex.Message,
                StackTrace = ex.StackTrace,
                ExceptionType = ex.GetType().Name,
                Timestamp = DateTime.UtcNow,
                UserId = userId,
                CorrelationId = correlationId,
                RequestPath = requestPath
            };
        }
    }
}
