using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Graduation_Project.BLL.DTOs.Notification
{
    public class NotificationDto
    {
        public int NotificationId { get; set; }
        public string Title { get; set; }
       public string Message { get; set; }
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
