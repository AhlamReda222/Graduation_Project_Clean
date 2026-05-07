using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Graduation_Project.BLL.DTOs.Auth
{
    public class GoogleLoginDto
    {
        // الـ Token اللي بيجي من Google في الـ Frontend
        public string IdToken { get; set; }
    }
}