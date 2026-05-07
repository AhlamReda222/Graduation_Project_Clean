using Graduation_Project.DAL.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Graduation_Project.DAL.Models.Entities
{
    public class Brand
    {
        public int BrandId { get; set; }
        public int UserId { get; set; }
        public string BrandName { get; set; }
        public string Description { get; set; }
        public string LogoUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set; }

        public virtual ApplicationUser User { get; set; }  
        public virtual ICollection<Product> Products { get; set; }
    }
}