using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Graduation_Project.BLL.DTOs.Chat
{
    // إرسال رسالة جديدة
    public class SendMessageDto
    {
        public int ReceiverId { get; set; }
        public string MessageText { get; set; }
    }

    // رسالة واحدة
    public class MessageDto
    {
        public int MessageId { get; set; }
        public int ConversationId { get; set; }
        public int SenderId { get; set; }
        public string SenderName { get; set; }
        public string MessageText { get; set; }
        public DateTime SentAt { get; set; }
        public bool IsRead { get; set; }
    }

    // محادثة في القائمة
    public class ConversationDto
    {
        public int ConversationId { get; set; }
        public int OtherUserId { get; set; }
        public string OtherUserName { get; set; }
        public string LastMessage { get; set; }
        public DateTime? LastMessageAt { get; set; }
        public int UnreadCount { get; set; }
    }
}