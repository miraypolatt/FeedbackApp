using FeedbackApp.API.Middleware;
using FeedbackApp.Application.Interfaces;
using FeedbackApp.Application.Services;
using FeedbackApp.Infrastructure.Logging;
using FeedbackApp.Infrastructure.Messaging;
using FeedbackApp.Infrastructure.Persistence;
using FeedbackApp.Infrastructure.Persistence.Repositories;
using FeedbackApp.Infrastructure.Settings;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<MongoDbSettings>(
    builder.Configuration.GetSection("MongoDbSettings"));

var mongoSettings = builder.Configuration
    .GetSection("MongoDbSettings")
    .Get<MongoDbSettings>();

builder.Services.AddSingleton<IMongoClient>(
    _ => new MongoClient(mongoSettings?.ConnectionString));

builder.Services.AddSingleton<IMongoDatabase>(sp =>
{
    var client = sp.GetRequiredService<IMongoClient>();
    return client.GetDatabase(mongoSettings?.DatabaseName);
});

builder.Services.AddSingleton<MongoDbContext>();
builder.Services.AddScoped<IFeedbackService, FeedbackService>();
builder.Services.AddScoped<IFeedbackRepository, FeedbackRepository>();
builder.Services.AddScoped<IMessageQueuePublisher, RabbitMqPublisherService>();
builder.Services.AddSingleton<IErrorLogger, MongoErrorLogger>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins("http://localhost:5173") // React dev server portu
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseMiddleware<GlobalExceptionMiddleware>(); 


app.UseHttpsRedirection();
app.UseRouting();
app.UseCors("AllowReactApp");
app.UseAuthorization();


app.MapControllers();
app.Run();