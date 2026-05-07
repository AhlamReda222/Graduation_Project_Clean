using Graduation_Project.DAL.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Graduation_Project.DAL.Models.Entities
{
    public class Conversation
    {
        public int ConversationId { get; set; }
        public int CustomerId { get; set; }
        public int BrandOwnerId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastMessageAt { get; set; }

        // Navigation Properties
        public virtual ApplicationUser Customer { get; set; }
        public virtual ApplicationUser BrandOwner { get; set; }
        public virtual ICollection<Message> Messages { get; set; }
    }
}
