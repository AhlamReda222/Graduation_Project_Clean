using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Graduation_Project.BLL.Common;
using Graduation_Project.BLL.DTOs.Chat;

namespace Graduation_Project.BLL.Services.Interfaces
{
    public interface IChatService
    {
        Task<ServiceResult<int>> GetOrCreateConversationAsync(int customerId, int brandOwnerId);
        Task<ServiceResult<MessageDto>> SendMessageAsync(int senderId, SendMessageDto dto);
        Task<ServiceResult<IEnumerable<MessageDto>>> GetConversationMessagesAsync(int conversationId, int userId);
        Task<ServiceResult<IEnumerable<ConversationDto>>> GetMyConversationsAsync(int userId);
        Task<ServiceResult<bool>> MarkMessagesAsReadAsync(int conversationId, int userId);
    }
}