using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
namespace Graduation_Project.DAL.Models.Entities
{
    public class RefreshToken
    {
        public int Id { get; set; }

        public string Token { get; set; } = null!;

        public DateTime Expires { get; set; }

        public bool IsRevoked { get; set; }

        public int ApplicationUserId { get; set; }

        public ApplicationUser User { get; set; } = null!;
    }
}
