using CarpoolPlatformAPI.CustomActionFilters;
using CarpoolPlatformAPI.Models.DTO.Message;
using CarpoolPlatformAPI.Services.IService;
using CarpoolPlatformAPI.Util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace CarpoolPlatformAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class MessagesController : ControllerBase
    {
        private readonly IMessageService _messageService;

        public MessagesController(IMessageService messageService)
        {
            _messageService = messageService;
        }


        [HttpGet]
        [Route("inbox/{id}")]
        public async Task<IActionResult> GetAllConversationsForUser([FromRoute] string id)
        {
            var serviceResponse = await _messageService.GetAllConversationsForUser(id);
            return ValidationService.HandleServiceResponse(serviceResponse);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllConversationMessages([FromQuery] string userOne, [FromQuery] string userTwo)
        {
            var serviceResponse = await _messageService.GetAllConversationMessagesAsync(userOne, userTwo, 
                includeProperties: "Sender, Receiver");
            return ValidationService.HandleServiceResponse(serviceResponse);
        }

        [HttpGet]
        [Route("{id:int}")]
        public async Task<IActionResult> GetMessageById([FromRoute] int id)
        {
            var serviceResponse = await _messageService.GetMessageAsync(m => m.Id == id, includeProperties: "Sender, Receiver");
            return ValidationService.HandleServiceResponse(serviceResponse);
        }

        [HttpPost]
        [ValidateModel]
        public async Task<IActionResult> CreateMessage([FromBody] MessageCreateDTO messageCreateDTO)
        {
            var serviceResponse = await _messageService.CreateMessageAsync(messageCreateDTO);
            return ValidationService.HandleServiceResponse(serviceResponse);
        }

        [HttpPut]
        [Route("{id:int}")]
        [ValidateModel]
        public async Task<IActionResult> UpdateMessage([FromRoute] int id, [FromBody] MessageUpdateDTO messageUpdateDTO)
        {
            var serviceResponse = await _messageService.UpdateMessageAsync(id, messageUpdateDTO);
            return ValidationService.HandleServiceResponse(serviceResponse);
        }

        [HttpPut]
        [Route("chat")]
        [ValidateModel]
        public async Task<IActionResult> MarkConversationMessagesAsRead([FromQuery] string userId, [FromQuery] string otherUserId)
        {
            var serviceResponse = await _messageService.MarkConversationMessagesAsRead(userId, otherUserId);
            return ValidationService.HandleServiceResponse(serviceResponse);
        }
    }
}
