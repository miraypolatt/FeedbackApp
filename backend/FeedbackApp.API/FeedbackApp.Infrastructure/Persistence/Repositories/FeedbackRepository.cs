using FeedbackApp.Application.Interfaces;
using FeedbackApp.Domain.Entities;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FeedbackApp.Infrastructure.Persistence.Repositories
{
    public class FeedbackRepository : IFeedbackRepository
    {
        private readonly IMongoCollection<Feedback> _collection;

        public FeedbackRepository(IMongoDatabase database)
        {
            _collection = database.GetCollection<Feedback>("feedbacks");
        }

        public async Task CreateAsync(Feedback feedback)
        {
            await _collection.InsertOneAsync(feedback);
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var filter = Builders<Feedback>.Filter.Eq(f => f.Id, Guid.Parse(id));
            var result = await _collection.DeleteOneAsync(filter);
            return result.DeletedCount > 0;
        }

        public async Task<List<Feedback>> GetAllAsync()
        {
            return await _collection.Find(_ => true).ToListAsync();
        }

        public async Task<Feedback?> GetByIdAsync(string id)
        {
            try
            {
                var filter = Builders<Feedback>.Filter.Eq(f => f.Id, Guid.Parse(id));
                return await _collection.Find(filter).FirstOrDefaultAsync();
            }
            catch (FormatException ex)
            {
                throw new ArgumentException($"Id değeri geçerli bir Guid formatında değil: {id}", ex);
            }
        }

        public async Task<bool> UpdateAsync(string id, Feedback feedback)
        {
            var filter = Builders<Feedback>.Filter.Eq(f => f.Id, Guid.Parse(id));
            var update = Builders<Feedback>.Update
                .Set(f => f.Name, feedback.Name)
                .Set(f => f.Email, feedback.Email)
                .Set(f => f.Message, feedback.Message)
                .Set(f => f.SubmittedAt, DateTime.UtcNow);
            var result = await _collection.UpdateOneAsync(filter, update);
            return result.ModifiedCount > 0;
        }
    }
}
