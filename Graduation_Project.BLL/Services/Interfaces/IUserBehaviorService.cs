using Graduation_Project.BLL.Common;
using Graduation_Project.BLL.DTOs.Tracking;

namespace Graduation_Project.BLL.Services.Interfaces
{
    public interface IUserBehaviorService
    {
        Task<ServiceResult<bool>> TrackEventAsync(TrackUserBehaviorRequest request, int? userId);
    }
}