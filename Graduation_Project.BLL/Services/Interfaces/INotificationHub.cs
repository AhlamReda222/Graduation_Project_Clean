using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Graduation_Project.BLL.Services.Interfaces
{
    public interface INotificationHub
    {
        Task SendNotificationAsync(int userId, object notification);
    }
}