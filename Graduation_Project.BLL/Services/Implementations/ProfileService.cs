using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Graduation_Project.BLL.Common;
using Graduation_Project.BLL.DTOs.Profile;
using Graduation_Project.BLL.Services.Interfaces;
using Graduation_Project.DAL.Models.Entities;
using Graduation_Project.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Graduation_Project.BLL.Services.Implementations
{
    public class ProfileService : IProfileService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProfileService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // ── جلب الـ Profile ──
        public async Task<ServiceResult<ProfileDto>> GetMyProfileAsync(int userId)
        {
            try
            {
                var profile = await _unitOfWork.Profiles
                    .GetQueryable()
                    .Include(p => p.User)
                    .FirstOrDefaultAsync(p => p.UserId == userId);

                if (profile == null)
                    return ServiceResult<ProfileDto>.Failure("Profile not found.");

                return ServiceResult<ProfileDto>.Success(MapToDto(profile));
            }
            catch (Exception ex)
            {
                return ServiceResult<ProfileDto>.Failure($"Error: {ex.Message}");
            }
        }

        // ── تعديل الـ Profile ──
        public async Task<ServiceResult<ProfileDto>> UpdateProfileAsync(int userId, UpdateProfileDto dto)
        {
            try
            {
                var profile = await _unitOfWork.Profiles
                    .GetQueryable()
                    .Include(p => p.User)
                    .FirstOrDefaultAsync(p => p.UserId == userId);

                if (profile == null)
                    return ServiceResult<ProfileDto>.Failure("Profile not found.");

                // تحديث الـ Profile
                if (!string.IsNullOrEmpty(dto.Address))
                    profile.Address = dto.Address;

                if (!string.IsNullOrEmpty(dto.Bio))
                    profile.Bio = dto.Bio;

                profile.UpdatedAt = DateTime.UtcNow;

                // تحديث الـ FullName في ApplicationUser
                if (!string.IsNullOrEmpty(dto.FullName) && profile.User != null)
                {
                    profile.User.FullName = dto.FullName;
                    profile.User.UpdatedAt = DateTime.UtcNow;
                    _unitOfWork.ApplicationUsers.Update(profile.User);
                }

                _unitOfWork.Profiles.Update(profile);
                await _unitOfWork.SaveAsync();

                return ServiceResult<ProfileDto>.Success(MapToDto(profile));
            }
            catch (Exception ex)
            {
                return ServiceResult<ProfileDto>.Failure($"Error: {ex.Message}");
            }
        }

        // ── تغيير صورة الـ Profile ──
        public async Task<ServiceResult<ProfileDto>> UpdateProfileImageAsync(int userId, UpdateProfileImageDto dto)
        {
            try
            {
                var profile = await _unitOfWork.Profiles
                    .GetQueryable()
                    .Include(p => p.User)
                    .FirstOrDefaultAsync(p => p.UserId == userId);

                if (profile == null)
                    return ServiceResult<ProfileDto>.Failure("Profile not found.");

                profile.ProfileImage = dto.ProfileImage;
                profile.UpdatedAt = DateTime.UtcNow;

                _unitOfWork.Profiles.Update(profile);
                await _unitOfWork.SaveAsync();

                return ServiceResult<ProfileDto>.Success(MapToDto(profile));
            }
            catch (Exception ex)
            {
                return ServiceResult<ProfileDto>.Failure($"Error: {ex.Message}");
            }
        }

        private ProfileDto MapToDto(Profile profile) => new ProfileDto
        {
            ProfileId = profile.ProfileId,
            UserId = profile.UserId,
            FullName = profile.User?.FullName,
            Email = profile.User?.Email,
            ProfileImage = profile.ProfileImage,
            Address = profile.Address,
            Bio = profile.Bio,
            UserType = profile.User?.UserType.ToString(),
            UpdatedAt = profile.UpdatedAt
        };
    }
}