using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Graduation_Project.BLL.Common;
using Graduation_Project.BLL.DTOs.Email;

namespace Graduation_Project.BLL.Services.Interfaces
{
    public interface IInAppEmailService
    {
        Task<ServiceResult<IEnumerable<InAppEmailDto>>> GetMyEmailsAsync(int userId);
        Task<ServiceResult<InAppEmailDto>> GetEmailByIdAsync(int emailId, int userId);
        Task<ServiceResult<bool>> MarkAsReadAsync(int emailId, int userId);
        Task<ServiceResult<bool>> MarkAllAsReadAsync(int userId);
        Task<ServiceResult<int>> GetUnreadCountAsync(int userId);
    }
}