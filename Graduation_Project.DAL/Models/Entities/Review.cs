using Graduation_Project.DAL.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Graduation_Project.DAL.Models.Entities
{
    public class Review
    {
        public int ReviewId { get; set; }
        public int ProductId { get; set; }
        public int UserId { get; set; }
        public int OrderId { get; set; }
        public int Rating { get; set; }
        public string ReviewText { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsDeleted { get; set; }
        public int? DeletedBy { get; set; }

        // Navigation Properties
        public virtual Product Product { get; set; }
        public virtual ApplicationUser User { get; set; }
        public virtual Order Order { get; set; }
        public virtual ApplicationUser DeletedByUser { get; set; }
    }

}
