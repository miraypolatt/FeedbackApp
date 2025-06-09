using FeedbackApp.Application.Interfaces;
using FeedbackApp.Domain.Entities;
using FeedbackApp.Infrastructure.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FeedbackApp.Infrastructure.Persistence
{
    public class MongoDbContext
    {
        private readonly IMongoDatabase _database;
        private readonly ILogger<MongoDbContext> _logger;

        public MongoDbContext(IConfiguration configuration, ILogger<MongoDbContext> logger)
        {
            _logger = logger;

            try
            {
                var connectionString = configuration["MongoDbSettings:ConnectionString"];
                var databaseName = configuration["MongoDbSettings:DatabaseName"];

                if (string.IsNullOrWhiteSpace(connectionString))
                    throw new ArgumentNullException(nameof(connectionString), "MongoDB bağlantı cümlesi boş!");

                if (string.IsNullOrWhiteSpace(databaseName))
                    throw new ArgumentNullException(nameof(databaseName), "MongoDB veritabanı adı boş!");

                var client = new MongoClient(connectionString);
                _database = client.GetDatabase(databaseName);

                _logger.LogInformation("MongoDB bağlantısı başarıyla kuruldu.");
                _logger.LogInformation("Veritabanı adı: {DatabaseName}", databaseName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "MongoDB bağlantısı kurulurken bir hata oluştu.");
                throw;
            }
        }
    }
}
