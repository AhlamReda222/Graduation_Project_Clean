using Graduation_Project.BLL.DTOs.Chat;
using Graduation_Project.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace Graduation_Project.Api.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly IChatService _chatService;

        public ChatHub(IChatService chatService)
        {
            _chatService = chatService;
        }

        // عند الاتصال: يضاف لـ Group باسم userId
        public override async Task OnConnectedAsync()
        {
            var userId = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!string.IsNullOrEmpty(userId))
                await Groups.AddToGroupAsync(Context.ConnectionId, $"user_{userId}");

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var userId = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!string.IsNullOrEmpty(userId))
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"user_{userId}");

            await base.OnDisconnectedAsync(exception);
        }

        // Client يبعت: hub.invoke("SendMessage", { receiverId, messageText })
        public async Task SendMessage(SendMessageDto dto)
        {
            var senderIdStr = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(senderIdStr))
            {
                await Clients.Caller.SendAsync("Error", "Unauthorized.");
                return;
            }

            var senderId = int.Parse(senderIdStr);
            var result = await _chatService.SendMessageAsync(senderId, dto);

            if (!result.Succeeded)
            {
                await Clients.Caller.SendAsync("Error", result.Errors.FirstOrDefault());
                return;
            }

            // إرسال للـ sender والـ receiver في نفس الوقت
            await Clients.Group($"user_{senderId}").SendAsync("ReceiveMessage", result.Data);
            await Clients.Group($"user_{dto.ReceiverId}").SendAsync("ReceiveMessage", result.Data);
        }

        // Client يبعت: hub.invoke("MarkAsRead", conversationId)
        public async Task MarkAsRead(int conversationId)
        {
            var userIdStr = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdStr)) return;

            var userId = int.Parse(userIdStr);
            await _chatService.MarkMessagesAsReadAsync(conversationId, userId);

            await Clients.Group($"user_{userId}").SendAsync("MessagesRead", conversationId);
        }
    }
}