using AutoMapper;
using CarpoolPlatformAPI.Models.Domain;
using CarpoolPlatformAPI.Models.DTO.Booking;
using CarpoolPlatformAPI.Models.DTO.Message;
using CarpoolPlatformAPI.Models.DTO.Ride;
using CarpoolPlatformAPI.Repositories;
using CarpoolPlatformAPI.Repositories.IRepository;
using CarpoolPlatformAPI.Services.IService;
using CarpoolPlatformAPI.Util;
using System.Linq.Expressions;

namespace CarpoolPlatformAPI.Services
{
    public class MessageService : IMessageService
    {
        private readonly IMessageRepository _messageRepository;
        private readonly IUserRepository _userRepository;
        private readonly INotificationRepository _notificationRepository;
        private readonly IMapper _mapper;
        private readonly IValidationService _validationService;

        public MessageService(IMessageRepository messageRepository, IUserRepository userRepository, 
            INotificationRepository notificationRepository, IMapper mapper, IValidationService validationService)
        {
            _messageRepository = messageRepository;
            _userRepository = userRepository;
            _notificationRepository = notificationRepository;
            _mapper = mapper;
            _validationService = validationService;
        }

        public async Task<List<MessageDTO>> GetAllMessagesAsync(Expression<Func<Message, bool>>? filter = null,
            string? includeProperties = null, bool? notTracked = null)
        {
            var messages = await _messageRepository.GetAllAsync(filter, includeProperties, 2000, 1, notTracked);

            return _mapper.Map<List<MessageDTO>>(messages);
        }

        public async Task<MessageDTO?> GetMessageAsync(Expression<Func<Message, bool>>? filter = null, 
            string? includeProperties = null, bool? notTracked = null)
        {
            var message = await _messageRepository.GetAsync(filter, includeProperties, notTracked);

            return _mapper.Map<MessageDTO>(message);
        }

        public async Task<MessageDTO?> CreateMessageAsync(MessageCreateDTO messageCreateDTO)
        {
            var message = _mapper.Map<Message>(messageCreateDTO);
            message.CreatedAt = DateTime.Now;

            var sender = await _userRepository.GetAsync(u => u.Id == messageCreateDTO.SenderId);
            var receiver = await _userRepository.GetAsync(u => u.Id == messageCreateDTO.ReceiverId);

            if (sender  == null || receiver == null)
            {
                return null;
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

            return _mapper.Map<MessageDTO>(message);
        }

        public async Task<MessageDTO?> UpdateMessageAsync(int id, MessageUpdateDTO messageUpdateDTO)
        {
            var message = await _messageRepository.GetAsync(m => m.Id == id && m.DeletedAt == null);

            if (message == null || message.SenderId != _validationService.GetCurrentUserId())
            {
                return null;
            }

            if(messageUpdateDTO.Content == "This message has been deleted by the user.")
            {
                message.DeletedAt = DateTime.Now;
            } else
            {
                message.UpdatedAt = DateTime.Now;
            }

            message = await _messageRepository.UpdateAsync(_mapper.Map<Message>(messageUpdateDTO));

            return _mapper.Map<MessageDTO>(message);
        }
    }
}
