using Graduation_Project.DAL.Models.Entities;
using Graduation_Project.DAL.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Graduation_Project.DAL.Models.Entities
{
    public class BrandOwnerRequest
    {
        public int RequestId { get; set; }
        public int UserId { get; set; }
        public string BusinessName { get; set; }
        public string BusinessLicense { get; set; }

        // ✅ بيانات البراند
        public string BrandName { get; set; }
        public string BrandDescription { get; set; }
        public string BrandLogoUrl { get; set; }

        public RequestStatus RequestStatus { get; set; }
        public DateTime RequestDate { get; set; }
        public int? ReviewedBy { get; set; }
        public DateTime? ReviewDate { get; set; }

        public virtual ApplicationUser User { get; set; }
        public virtual ApplicationUser Reviewer { get; set; }
    }
}