using FeedbackApp.Application.Interfaces;
using FeedbackApp.Domain.Entities;
using FeedbackApp.Infrastructure.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;

namespace FeedbackApp.Infrastructure.Logging
{
    public class MongoErrorLogger : IErrorLogger
    {
        private readonly IMongoCollection<ErrorLog> _errorLogs;
        private readonly ILogger<MongoErrorLogger> _logger;
        public MongoErrorLogger(IOptions<MongoDbSettings> settings, ILogger<MongoErrorLogger> logger)
        {
           _logger = logger;

            try
            {
                var connStr = settings.Value.ConnectionString;
                var dbName = settings.Value.DatabaseName;
                if (string.IsNullOrEmpty(connStr) || string.IsNullOrEmpty(dbName))
                {
                    _logger.LogError("MongoErrorLogger: MongoDbSettings boş geldi. ConnectionString: '{ConnectionString}', Database: '{DatabaseName}'", connStr, dbName);
                    throw new InvalidOperationException("MongoDbSettings boş geldi.");
                }

                var client = new MongoClient(connStr);
                var database = client.GetDatabase(dbName);
                _errorLogs = database.GetCollection<ErrorLog>("error_logs");

                _logger.LogInformation("MongoErrorLogger initialized with DB: {Db}", settings.Value.DatabaseName);
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "MongoErrorLogger initialization failed. Check MongoDbSettings.");
                throw; 
            }
        }

        public async Task LogAsync(ErrorLog errorLog)
        {
            try
            {
                Console.WriteLine("➡ MongoErrorLogger.LogAsync() invoked");
                await _errorLogs.InsertOneAsync(errorLog);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ MongoDB error log insert failed. Logging to Console fallback...");


                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("=== MONGO LOGGING FAILED ===");
                Console.WriteLine($"Message: {errorLog.Message}");
                Console.WriteLine($"StackTrace: {errorLog.StackTrace}");
                Console.WriteLine($"Original Exception: {ex.Message}");
                Console.ResetColor();
            }
        }
    }
}
