using AutoMapper;
using CarpoolPlatformAPI.Models.Domain;
using CarpoolPlatformAPI.Models.DTO.Message;
using CarpoolPlatformAPI.Repositories.IRepository;
using CarpoolPlatformAPI.Services.IService;
using CarpoolPlatformAPI.Util;
using CarpoolPlatformAPI.Util.IValidation;
using System.Linq.Expressions;
using System.Net;

namespace CarpoolPlatformAPI.Services
{
    public class MessageService : IMessageService
    {
        private readonly IMessageRepository _messageRepository;
        private readonly IUserRepository _userRepository;
        private readonly INotificationRepository _notificationRepository;
        private readonly IValidationService _validationService;
        private readonly IMapper _mapper;

        public MessageService(IMessageRepository messageRepository, IUserRepository userRepository, 
            INotificationRepository notificationRepository, IValidationService validationService, IMapper mapper)
        {
            _messageRepository = messageRepository;
            _userRepository = userRepository;
            _notificationRepository = notificationRepository;
            _validationService = validationService;
            _mapper = mapper;
        }

        public async Task<ServiceResponse<List<MessageDTO>>> GetAllConversationMessagesAsync(string userOneId, string userTwoId,
            string? includeProperties = null, bool? notTracked = null)
        {
            if (_validationService.GetCurrentUserId() != userOneId || _validationService.GetCurrentUserId() != userTwoId)
            {
                return new ServiceResponse<List<MessageDTO>>(HttpStatusCode.Forbidden, "You are not allowed to access this information.");
            }

            var messages = await _messageRepository.GetAllAsync(
                m => (m.SenderId == userOneId && m.ReceiverId == userTwoId) ||
                     (m.SenderId == userTwoId && m.ReceiverId == userOneId),
                     includeProperties, 2500, 1, notTracked);

            return new ServiceResponse<List<MessageDTO>>(HttpStatusCode.OK, _mapper.Map<List<MessageDTO>>(messages));
        }

        public async Task<ServiceResponse<MessageDTO?>> GetMessageAsync(Expression<Func<Message, bool>>? filter = null, 
            string? includeProperties = null, bool? notTracked = null)
        {
            var message = await _messageRepository.GetAsync(filter, includeProperties, notTracked);

            if (message == null)
            {
                return new ServiceResponse<MessageDTO?>(HttpStatusCode.NotFound, "The message has not been found.");
            }
            else if (_validationService.GetCurrentUserId() != message.SenderId || 
                     _validationService.GetCurrentUserId() != message.ReceiverId)
            {
                return new ServiceResponse<MessageDTO?>(HttpStatusCode.Forbidden, "You are not allowed to access this information.");
            }

            return new ServiceResponse<MessageDTO?>(HttpStatusCode.OK, _mapper.Map<MessageDTO>(message));
        }

        public async Task<ServiceResponse<MessageDTO?>> CreateMessageAsync(MessageCreateDTO messageCreateDTO)
        {
            var message = _mapper.Map<Message>(messageCreateDTO);
            message.CreatedAt = DateTime.Now;
            var sender = await _userRepository.GetAsync(u => u.Id == messageCreateDTO.SenderId && u.DeletedAt == null);
            var receiver = await _userRepository.GetAsync(u => u.Id == messageCreateDTO.ReceiverId && u.DeletedAt == null);

            if (sender  == null)
            {
                return new ServiceResponse<MessageDTO?>(HttpStatusCode.NotFound, "The sender has not been found.");
            }
            else if (receiver == null)
            {
                return new ServiceResponse<MessageDTO?>(HttpStatusCode.NotFound, "The receiver has not been found.");
            }
            else if (_validationService.GetCurrentUserId() != messageCreateDTO.SenderId)
            {
                return new ServiceResponse<MessageDTO?>(HttpStatusCode.Forbidden, "You are not allowed to send this message.");
            }

            sender.SentMessages.Add(message);
            receiver.ReceivedMessages.Add(message);
            sender.UpdatedAt = DateTime.Now;
            message = await _messageRepository.CreateAsync(message);

            var notification = new Notification
            {
                Message = $"You have received a message from {sender.FirstName} ${sender.LastName}.",
                UserId = receiver.Id,
                CreatedAt = DateTime.Now
            };
            receiver.Notifications.Add(notification);
            receiver.UpdatedAt = DateTime.Now;
            await _notificationRepository.CreateAsync(notification);

            return new ServiceResponse<MessageDTO?>(HttpStatusCode.Created, _mapper.Map<MessageDTO>(message));
        }

        public async Task<ServiceResponse<MessageDTO?>> UpdateMessageAsync(int id, MessageUpdateDTO messageUpdateDTO)
        {
            var message = await _messageRepository.GetAsync(m => m.Id == id && m.DeletedAt == null);

            if (message == null)
            {
                return new ServiceResponse<MessageDTO?>(HttpStatusCode.NotFound, "The message has not been found.");
            }
            else if (_validationService.GetCurrentUserId() != message.SenderId)
            {
                return new ServiceResponse<MessageDTO?>(HttpStatusCode.Forbidden, "You are not allowed to edit or remove this message.");
            }

            if(messageUpdateDTO.Content == "This message has been deleted by the user.")
            {
                message.DeletedAt = DateTime.Now;
            }
            message.UpdatedAt = DateTime.Now;
            message = await _messageRepository.UpdateAsync(_mapper.Map<Message>(messageUpdateDTO));

            return new ServiceResponse<MessageDTO?>(HttpStatusCode.OK, _mapper.Map<MessageDTO>(message));
        }
    }
}
