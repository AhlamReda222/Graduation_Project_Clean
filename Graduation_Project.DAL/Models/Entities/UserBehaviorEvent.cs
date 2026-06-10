// FILE: DAL/Models/Entities/UserBehaviorEvent.cs
using Graduation_Project.DAL.Enums;

namespace Graduation_Project.DAL.Models.Entities
{
    public class UserBehaviorEvent
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        // ✅ nullable - لو مش مسجل دخول بيبقى null
        public int? UserId { get; set; }

        // ✅ دايماً موجود - بيتعمل تلقائي من الـ Middleware
        public string SessionId { get; set; } = null!;

        public UserBehaviorEventType EventType { get; set; }

        public int? ProductId  { get; set; }
        public int? CategoryId { get; set; }
        public int? BrandId    { get; set; }

        public string? SearchQuery { get; set; }
        public string? SourcePage  { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // ✅ Navigation - ربط اختياري بالـ User
        // لو UserId = null مش هيعمل مشكلة
        public virtual ApplicationUser? User { get; set; }
    }
}