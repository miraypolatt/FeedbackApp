using FeedbackApp.Application.Interfaces;
using FeedbackApp.Consumer;
using FeedbackApp.Infrastructure.Logging;
using FeedbackApp.Infrastructure.Messaging;
using FeedbackApp.Infrastructure.Persistence.Repositories;
using FeedbackApp.Infrastructure.Settings;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((hostingContext, config) =>
    {
        config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
    })
    .ConfigureServices((context, services) =>
    {

        services.Configure<FeedbackApp.Infrastructure.Settings.MongoDbSettings>(
            context.Configuration.GetSection("MongoDbSettings"));

 
        services.AddSingleton<IMongoClient>(sp =>
        {
            var mongoOptions = sp.GetRequiredService<IOptions<FeedbackApp.Infrastructure.Settings.MongoDbSettings>>();
            return new MongoClient(mongoOptions.Value.ConnectionString);
        });


        services.AddSingleton<IMongoDatabase>(sp =>
        {
            var mongoOptions = sp.GetRequiredService<IOptions<FeedbackApp.Infrastructure.Settings.MongoDbSettings>>();
            var client = sp.GetRequiredService<IMongoClient>();
            return client.GetDatabase(mongoOptions.Value.DatabaseName);
        });

        services.AddScoped<IErrorLogger, MongoErrorLogger>();

        services.AddScoped<IFeedbackRepository, FeedbackRepository>();
        services.AddScoped<IFeedbackProcessor, FeedbackProcessor>();

        services.AddHostedService<Worker>();
    })
    .Build()
    .Run();
