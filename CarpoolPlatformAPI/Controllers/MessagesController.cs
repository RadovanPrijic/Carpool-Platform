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
using System.Reflection;

namespace CarpoolPlatformAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MessagesController : ControllerBase
    {
        private readonly IMessageService _messageService;

        public MessagesController(IMessageService messageService)
        {
            _messageService = messageService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllConversationMessages([FromQuery] string userOne, [FromQuery] string userTwo)
        {
            var serviceResponse = await _messageService.GetAllConversationMessagesAsync(userOne, userTwo);
            return ValidationService.HandleServiceResponse(serviceResponse);
        }

        [HttpGet]
        [Route("{id:int}")]
        public async Task<IActionResult> GetMessageById([FromRoute] int id)
        {
            var serviceResponse = await _messageService.GetMessageAsync(m => m.Id == id);
            return ValidationService.HandleServiceResponse(serviceResponse);
        }

        [HttpPost]
        [ValidateModel]
        public async Task<IActionResult> CreateMessage([FromBody] MessageCreateDTO messageCreateDTO)
        {
            var serviceResponse = await _messageService.CreateMessageAsync(messageCreateDTO);
            return ValidationService.HandleServiceResponse(serviceResponse, this, nameof(GetMessageById), new { id = serviceResponse.Data!.Id });
        }

        [HttpPut]
        [Route("{id:int}")]
        [ValidateModel]
        public async Task<IActionResult> UpdateMessage([FromRoute] int id, [FromBody] MessageUpdateDTO messageUpdateDTO)
        {
            var serviceResponse = await _messageService.UpdateMessageAsync(id, messageUpdateDTO);
            return ValidationService.HandleServiceResponse(serviceResponse);
        }
    }
}
