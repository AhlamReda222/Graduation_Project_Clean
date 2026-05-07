using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Graduation_Project.BLL.Common;
using Graduation_Project.BLL.DTOs.Email;
using Graduation_Project.BLL.Services.Interfaces;
using Graduation_Project.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Graduation_Project.BLL.Services.Implementations
{
    public class InAppEmailService : IInAppEmailService
    {
        private readonly IUnitOfWork _unitOfWork;

        public InAppEmailService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // جلب كل الإيميلات بتاعت اليوزر
        public async Task<ServiceResult<IEnumerable<InAppEmailDto>>> GetMyEmailsAsync(int userId)
        {
            try
            {
                var emails = await _unitOfWork.InAppEmails
                    .GetQueryable()
                    .Where(e => e.UserId == userId)
                    .OrderByDescending(e => e.CreatedAt)
                    .ToListAsync();

                return ServiceResult<IEnumerable<InAppEmailDto>>.Success(
                    emails.Select(MapToDto));
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<InAppEmailDto>>.Failure($"Error: {ex.Message}");
            }
        }

        // فتح إيميل معين (زي ما تفتح إيميل في Gmail)
        public async Task<ServiceResult<InAppEmailDto>> GetEmailByIdAsync(int emailId, int userId)
        {
            try
            {
                var email = await _unitOfWork.InAppEmails
                    .FindOneAsync(e => e.EmailId == emailId && e.UserId == userId);

                if (email == null)
                    return ServiceResult<InAppEmailDto>.Failure("Email not found.");

                // لما يفتحه يتعلم كمقروء تلقائياً
                if (!email.IsRead)
                {
                    email.IsRead = true;
                    _unitOfWork.InAppEmails.Update(email);
                    await _unitOfWork.SaveAsync();
                }

                return ServiceResult<InAppEmailDto>.Success(MapToDto(email));
            }
            catch (Exception ex)
            {
                return ServiceResult<InAppEmailDto>.Failure($"Error: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> MarkAsReadAsync(int emailId, int userId)
        {
            try
            {
                var email = await _unitOfWork.InAppEmails
                    .FindOneAsync(e => e.EmailId == emailId && e.UserId == userId);

                if (email == null)
                    return ServiceResult<bool>.Failure("Email not found.");

                email.IsRead = true;
                _unitOfWork.InAppEmails.Update(email);
                await _unitOfWork.SaveAsync();

                return ServiceResult<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return ServiceResult<bool>.Failure($"Error: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> MarkAllAsReadAsync(int userId)
        {
            try
            {
                var unread = await _unitOfWork.InAppEmails
                    .FindAsync(e => e.UserId == userId && !e.IsRead);

                foreach (var email in unread)
                {
                    email.IsRead = true;
                    _unitOfWork.InAppEmails.Update(email);
                }

                await _unitOfWork.SaveAsync();

                return ServiceResult<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return ServiceResult<bool>.Failure($"Error: {ex.Message}");
            }
        }

        public async Task<ServiceResult<int>> GetUnreadCountAsync(int userId)
        {
            try
            {
                var count = await _unitOfWork.InAppEmails
                    .GetQueryable()
                    .CountAsync(e => e.UserId == userId && !e.IsRead);

                return ServiceResult<int>.Success(count);
            }
            catch (Exception ex)
            {
                return ServiceResult<int>.Failure($"Error: {ex.Message}");
            }
        }

        private InAppEmailDto MapToDto(DAL.Models.Entities.InAppEmail e) => new InAppEmailDto
        {
            EmailId = e.EmailId,
            Subject = e.Subject,
            Body = e.Body,
            IsRead = e.IsRead,
            CreatedAt = e.CreatedAt
        };
    }
}
