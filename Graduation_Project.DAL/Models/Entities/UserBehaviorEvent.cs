using Graduation_Project.DAL.Models.Entities;
using Graduation_Project.DAL.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Graduation_Project.DAL.Enums;

namespace Graduation_Project.DAL.Models.Entities
{
    public class UserBehaviorEvent
    {
        public Guid Id { get; set; }

        public int? UserId { get; set; }

        public string SessionId { get; set; } = null!;

        public UserBehaviorEventType EventType { get; set; }

        public int? ProductId { get; set; }

        public int? CategoryId { get; set; }

        public int? BrandId { get; set; }

        public string? SearchQuery { get; set; }

        public string? SourcePage { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}