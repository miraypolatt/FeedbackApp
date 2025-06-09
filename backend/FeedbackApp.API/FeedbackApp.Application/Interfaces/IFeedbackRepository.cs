using FeedbackApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FeedbackApp.Application.Interfaces
{
    public interface IFeedbackRepository
    {
        Task CreateAsync(Feedback feedback);
        Task<bool> DeleteAsync(string id);
        Task<List<Feedback>> GetAllAsync();
        Task<Feedback?> GetByIdAsync(string id);
        Task<bool> UpdateAsync(string id, Feedback feedback);
    }
}
