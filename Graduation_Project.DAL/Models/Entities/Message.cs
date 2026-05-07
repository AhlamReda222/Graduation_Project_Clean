using Graduation_Project.DAL.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Graduation_Project.DAL.Models.Entities
{
    public class Message
    {
        public int MessageId { get; set; }
        public int ConversationId { get; set; }
        public int SenderId { get; set; }
        public string MessageText { get; set; }
        public DateTime SentAt { get; set; }
        public bool IsRead { get; set; }

        // Navigation Properties
        public virtual Conversation Conversation { get; set; }
        public virtual ApplicationUser Sender { get; set; }
    }

}
