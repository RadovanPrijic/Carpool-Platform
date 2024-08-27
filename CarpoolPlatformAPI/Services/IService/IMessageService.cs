using CarpoolPlatformAPI.Models.Domain;
using CarpoolPlatformAPI.Models.DTO.Message;
using CarpoolPlatformAPI.Util;
using System.Linq.Expressions;

namespace CarpoolPlatformAPI.Services.IService
{
    public interface IMessageService
    {
        Task<ServiceResponse<List<MessageDTO>>> GetAllConversationMessagesAsync(string userOneId, string userTwoId, 
            string? includeProperties = null, bool? notTracked = null);
        Task<ServiceResponse<MessageDTO?>> GetMessageAsync(Expression<Func<Message, bool>>? filter = null,
            string? includeProperties = null, bool? notTracked = null);
        Task<ServiceResponse<MessageDTO?>> CreateMessageAsync(MessageCreateDTO messageCreateDTO);
        Task<ServiceResponse<MessageDTO?>> UpdateMessageAsync(int id, MessageUpdateDTO messageUpdateDTO);
    }
}
