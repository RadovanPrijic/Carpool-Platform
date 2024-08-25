using CarpoolPlatformAPI.Models.Domain;
using CarpoolPlatformAPI.Models.DTO.Booking;
using CarpoolPlatformAPI.Models.DTO.Message;
using System.Linq.Expressions;

namespace CarpoolPlatformAPI.Services.IService
{
    public interface IMessageService
    {
        Task<List<MessageDTO>> GetAllMessagesAsync(Expression<Func<Message, bool>>? filter = null, 
            string? includeProperties = null, bool? notTracked = null);
        Task<MessageDTO?> GetMessageAsync(Expression<Func<Message, bool>>? filter = null,
            string? includeProperties = null, bool? notTracked = null);
        Task<MessageDTO?> CreateMessageAsync(MessageCreateDTO messageCreateDTO);
        Task<MessageDTO?> UpdateMessageAsync(int id, MessageUpdateDTO messageUpdateDTO);
    }
}
