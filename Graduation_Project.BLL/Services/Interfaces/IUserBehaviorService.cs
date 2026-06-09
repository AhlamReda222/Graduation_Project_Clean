// FILE: BLL/Services/Interfaces/IUserBehaviorService.cs
using Graduation_Project.BLL.Common;
using Graduation_Project.BLL.DTOs.Tracking;

namespace Graduation_Project.BLL.Services.Interfaces
{
    public interface IUserBehaviorService
    {
        Task<ServiceResult<bool>> TrackEventAsync(
            TrackUserBehaviorRequest request,
            int? userId);

        // ✅ ربط الـ events القديمة بالـ User بعد تسجيل الدخول
        Task LinkSessionToUserAsync(string sessionId, int userId);
    }
}