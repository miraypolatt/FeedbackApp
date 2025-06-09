using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.Threading.Tasks;
using FeedbackApp.Domain.Entities;

namespace FeedbackApp.Application.Interfaces
{
   public interface IErrorLogger
   {
        Task LogAsync(ErrorLog errorLog);
   }
}
