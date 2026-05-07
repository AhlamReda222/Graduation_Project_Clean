using Graduation_Project.DAL.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Graduation_Project.DAL.Models.Entities

    {
        public class Profile
        {
            public int ProfileId { get; set; }
            public int UserId { get; set; } 
            public string ProfileImage { get; set; }
            public string Address { get; set; }
            public string Bio { get; set; }
            public DateTime UpdatedAt { get; set; }

            // Navigation Property
            public virtual ApplicationUser User { get; set; }  
        }
    }
