using FeedbackApp.Application.Helpers;
using FeedbackApp.Application.Interfaces;
using FeedbackApp.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using System.Text.Json;

namespace FeedbackApp.API.Middleware
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }
        public async Task InvokeAsync(HttpContext context, IErrorLogger errorLogger)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception occurred.");

                var statusCode = StatusCodes.Status500InternalServerError;
                var isClientError = ex is ArgumentException or FormatException;

                if (isClientError)
                {
                    statusCode = StatusCodes.Status400BadRequest;
                }

                var errorLog = ErrorLogFactory.FromException(
                    ex,
                    source: "GlobalExceptionMiddleware",
                    userId: context.User?.Identity?.Name ?? "anonymous",
                    correlationId: context.TraceIdentifier,
                    requestPath: context.Request.Path
                );

                errorLog.AdditionalData = new Dictionary<string, string?>
        {
            { "Method", context.Request.Method },
            { "QueryString", context.Request.QueryString.ToString() },
            { "RemoteIp", context.Connection.RemoteIpAddress?.ToString() },
            { "StatusCode", statusCode.ToString() }
        };

                await SafeLogAsync(errorLogger, errorLog);

                if (!context.Response.HasStarted)
                {
                    context.Response.Clear();
                    context.Response.StatusCode = statusCode;
                    context.Response.ContentType = "application/json";

                    var response = isClientError
                        ? new { error = ex.Message }
                        : new { error = "An unexpected error occurred. Please try again later." };

                    await context.Response.WriteAsync(JsonSerializer.Serialize(response));
                }
            }
        }

        private async Task SafeLogAsync(IErrorLogger logger, ErrorLog log)
        {
            try
            {
                await logger.LogAsync(log);
            }
            catch (Exception logEx)
            {
                _logger.LogError(logEx, "Error occurred while logging the original exception.");
                
            }
        }
    }
}
