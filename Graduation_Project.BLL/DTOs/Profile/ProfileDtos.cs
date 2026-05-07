using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Graduation_Project.BLL.DTOs.Profile
{
    // عرض الـ Profile
    public class ProfileDto
    {
        public int ProfileId { get; set; }
        public int UserId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string ProfileImage { get; set; }
        public string Address { get; set; }
        public string Bio { get; set; }
        public string UserType { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    // تعديل الـ Profile
    public class UpdateProfileDto
    {
        public string FullName { get; set; }
        public string Address { get; set; }
        public string Bio { get; set; }
    }

    // تغيير صورة الـ Profile
    public class UpdateProfileImageDto
    {
        public string ProfileImage { get; set; }
    }
}