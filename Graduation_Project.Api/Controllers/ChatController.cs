using Graduation_Project.BLL.DTOs.Chat;
using Graduation_Project.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using Graduation_Project.Api.Hubs;

namespace Graduation_Project.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ChatController : ControllerBase
    {
        private readonly IChatService _chatService;
        private readonly IHubContext<ChatHub> _hubContext;

        public ChatController(IChatService chatService, IHubContext<ChatHub> hubContext)
        {
            _chatService = chatService;
            _hubContext = hubContext;
        }

        // GET: api/chat/conversations
        [HttpGet("conversations")]
        public async Task<IActionResult> GetMyConversations()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var result = await _chatService.GetMyConversationsAsync(userId);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok(result.Data);
        }

        // GET: api/chat/conversations/{conversationId}/messages
        [HttpGet("conversations/{conversationId}/messages")]
        public async Task<IActionResult> GetMessages(int conversationId)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var result = await _chatService.GetConversationMessagesAsync(conversationId, userId);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            await _chatService.MarkMessagesAsReadAsync(conversationId, userId);

            return Ok(result.Data);
        }

        // POST: api/chat/conversations/start
        [HttpPost("conversations/start")]
        public async Task<IActionResult> StartConversation([FromBody] int otherUserId)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var result = await _chatService.GetOrCreateConversationAsync(userId, otherUserId);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok(new { conversationId = result.Data });
        }

        // POST: api/chat/send
        // بعت رسالة عن طريق HTTP + SignalR real-time
        [HttpPost("send")]
        public async Task<IActionResult> SendMessage([FromBody] SendMessageDto dto)
        {
            var senderId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var result = await _chatService.SendMessageAsync(senderId, dto);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            // بعت الرسالة real-time للـ sender والـ receiver
            await _hubContext.Clients
                .Group($"user_{senderId}")
                .SendAsync("ReceiveMessage", result.Data);

            await _hubContext.Clients
                .Group($"user_{dto.ReceiverId}")
                .SendAsync("ReceiveMessage", result.Data);

            return Ok(result.Data);
        }
    }
}