using FeedbackApp.Application.DTOs;
using FeedbackApp.Domain.Entities;

namespace FeedbackApp.Application.Interfaces
{
    public interface IFeedbackService
    {
        Task<IEnumerable<Feedback>> GetAllAsync();
        Task<Feedback?> GetByIdAsync(string id);
        Task CreateAsync(Feedback feedback);
        Task<bool> UpdateAsync(string id, Feedback feedback);
        Task<bool> DeleteAsync(string id);
    }
}
