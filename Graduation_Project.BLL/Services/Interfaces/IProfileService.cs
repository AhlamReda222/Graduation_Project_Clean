using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Graduation_Project.BLL.Common;
using Graduation_Project.BLL.DTOs.Profile;

namespace Graduation_Project.BLL.Services.Interfaces
{
    public interface IProfileService
    {
        Task<ServiceResult<ProfileDto>> GetMyProfileAsync(int userId);
        Task<ServiceResult<ProfileDto>> UpdateProfileAsync(int userId, UpdateProfileDto dto);
        Task<ServiceResult<ProfileDto>> UpdateProfileImageAsync(int userId, UpdateProfileImageDto dto);
    }
}