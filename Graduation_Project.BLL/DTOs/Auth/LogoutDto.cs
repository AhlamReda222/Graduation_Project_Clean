using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Graduation_Project.DAL.Models.Enums;
using System;
namespace Graduation_Project.BLL.DTOs.Auth{
    public class LogoutDto
    {
        public string RefreshToken { get; set; } = null!;

    }
}
