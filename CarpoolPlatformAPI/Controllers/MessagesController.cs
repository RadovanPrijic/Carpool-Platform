using CarpoolPlatformAPI.CustomActionFilters;
using CarpoolPlatformAPI.Models.Domain;
using CarpoolPlatformAPI.Models.DTO.Booking;
using CarpoolPlatformAPI.Models.DTO.Message;
using CarpoolPlatformAPI.Services;
using CarpoolPlatformAPI.Services.IService;
using CarpoolPlatformAPI.Util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CarpoolPlatformAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MessagesController : ControllerBase
    {
        private readonly IMessageService _messageService;
        private readonly IValidationService _validationService;

        public MessagesController(IMessageService messageService, IValidationService validationService)
        {
            _messageService = messageService;
            _validationService = validationService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllConversationMessages([FromQuery] string sender, [FromQuery] string receiver)
        {
            var messageDTOs = await _messageService.GetAllMessagesAsync(
                m => m.SenderId == sender &&
                m.ReceiverId == receiver &&
                m.DeletedAt == null);

            var orderedMessageDTOs = messageDTOs.OrderBy(m => m.CreatedAt);

            return Ok(orderedMessageDTOs);
        }

        [HttpGet]
        [Route("{id:int}")]
        public async Task<IActionResult> GetMessageById([FromRoute] int id)
        {
            var messageDTO = await _messageService.GetMessageAsync(
                m => m.Id == id,
                includeProperties: "Sender, Receiver");

            if (messageDTO == null)
            {
                return NotFound(new { message = "The message has not been found." });
            }

            return Ok(messageDTO);
        }

        [HttpPost]
        [ValidateModel]
        public async Task<IActionResult> CreateMessage([FromBody] MessageCreateDTO messageCreateDTO)
        {
            var messageDTO = await _messageService.CreateMessageAsync(messageCreateDTO);

            return CreatedAtAction(nameof(GetMessageById), new { id = messageDTO.Id }, messageDTO);
        }

/*        [HttpPut]
        [Route("{id:int}")]
        [ValidateModel]
        public async Task<IActionResult> UpdateMessage([FromRoute] int id, [FromBody] MessageUpdateDTO messageUpdateDTO)
        {
            var messageDTO = await _messageService.UpdateMessageAsync(id, messageUpdateDTO);

            if (messageDTO == null)
            {
                return NotFound(new { message = "The message has not been found." });
            }

            return Ok(messageDTO);
        }*/

        [HttpDelete]
        [Route("{id:int}")]
        public async Task<IActionResult> DeleteMessage([FromRoute] int id)
        {
            var messageDTO = await _messageService.RemoveMessageAsync(id);

            if (messageDTO == null)
            {
                return NotFound(new { message = "The message has not been found." });
            }

            return Ok(messageDTO);
        }
    }
}
