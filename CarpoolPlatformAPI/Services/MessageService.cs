using AutoMapper;
using CarpoolPlatformAPI.Models.Domain;
using CarpoolPlatformAPI.Models.DTO.Booking;
using CarpoolPlatformAPI.Models.DTO.Message;
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
        private readonly IMapper _mapper;
        private readonly IValidationService _validationService;

        public MessageService(IMessageRepository messageRepository, IMapper mapper, IValidationService validationService)
        {
            _messageRepository = messageRepository;
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

        public async Task<MessageDTO> CreateMessageAsync(MessageCreateDTO messageCreateDTO)
        {
            var message = await _messageRepository.CreateAsync(_mapper.Map<Message>(messageCreateDTO));

            return _mapper.Map<MessageDTO>(message);
        }

/*        public async Task<MessageDTO?> UpdateMessageAsync(int id, MessageUpdateDTO messageUpdateDTO)
        {
            var message = await _messageRepository.GetAsync(m => m.Id == id && m.DeletedAt == null);

            if (message == null)
            {
                return null;
            }

            message = await _messageRepository.UpdateAsync(_mapper.Map<Message>(messageUpdateDTO));

            return _mapper.Map<MessageDTO>(message);
        }*/

        public async Task<MessageDTO?> RemoveMessageAsync(int id)
        {
            var message = await _messageRepository.GetAsync(m => m.Id == id);

            if (message == null)
            {
                return null;
            }

            message.DeletedAt = DateTime.Now;
            message = await _messageRepository.UpdateAsync(message);

            return _mapper.Map<MessageDTO>(message);
        }
    }
}
