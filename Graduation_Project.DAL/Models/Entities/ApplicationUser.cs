using Graduation_Project.DAL.Models;
using System;
using System.Collections.Generic;
using Graduation_Project.DAL.Models.Enums;
using Microsoft.AspNetCore.Identity;
namespace Graduation_Project.DAL.Models.Entities
{
    public class ApplicationUser : IdentityUser<int>
    {
        public string? FullName { get; set; }
        public UserType UserType { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool IsBlocked { get; set; }
         public bool HasAcceptedContract { get; set; } = false; // ✅ الجديد


        // Navigation Properties
        public virtual Profile Profile { get; set; }
        public virtual ICollection<BrandOwnerRequest> SubmittedRequests { get; set; }
        public virtual ICollection<BrandOwnerRequest> ReviewedRequests { get; set; }
        public virtual ICollection<Brand> Brands { get; set; }
        public virtual ICollection<CartItem> CartItems { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
        public virtual ICollection<Review> WrittenReviews { get; set; }
        public virtual ICollection<Review> DeletedReviews { get; set; }
        public virtual ICollection<Conversation> CustomerConversations { get; set; }
        public virtual ICollection<Conversation> BrandOwnerConversations { get; set; }
        public virtual ICollection<Message> Messages { get; set; }
        public virtual ICollection<Product> ApprovedProducts { get; set; }
        public virtual ICollection<UserBehaviorEvent> BehaviorEvents{ get; set; }

    }
}