using Graduation_Project.BLL.Common;
using Graduation_Project.BLL.DTOs.BrandOwnerRequest;
using Graduation_Project.BLL.Services.Interfaces;
using Graduation_Project.DAL.Models.Entities;
using Graduation_Project.DAL.Models.Enums;
using Graduation_Project.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
namespace Graduation_Project.BLL.Services.Implementations
{
    public class BrandOwnerRequestService : IBrandOwnerRequestService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFileService _fileService;
        private readonly IEmailSender _emailSender;
        private readonly NotificationHelper _notificationHelper;

        public BrandOwnerRequestService(
            IUnitOfWork unitOfWork,
            IFileService fileService,
            IEmailSender emailSender,
            NotificationHelper notificationHelper)
        {
            _unitOfWork = unitOfWork;
            _fileService = fileService;
            _emailSender = emailSender;
            _notificationHelper = notificationHelper;
        }

        public async Task<BrandOwnerRequestDto> CreateRequestAsync(int userId, CreateBrandOwnerRequestDto dto)
        {
            var user = await _unitOfWork.ApplicationUsers.GetByIdAsync(userId);
            if (user == null)
                throw new KeyNotFoundException("User not found");

            if (await HasPendingRequestAsync(userId))
                throw new InvalidOperationException("You already have a pending request");

            // ✅ رفع License
            string? licenseUrl = null;
            if (dto.BusinessLicense != null)
                licenseUrl = await _fileService.UploadFileAsync(dto.BusinessLicense, "licenses");

            // ✅ رفع Logo
            string? logoUrl = null;
            if (dto.BrandLogo != null)
                logoUrl = await _fileService.UploadFileAsync(dto.BrandLogo, "logos");

            var request = new BrandOwnerRequest
            {
                UserId = userId,
                BusinessName = dto.BusinessName,
                BusinessLicense = licenseUrl,
                BrandName = dto.BrandName,
                BrandDescription = dto.BrandDescription,
                BrandLogoUrl = logoUrl,
                RequestStatus = RequestStatus.Pending,
                RequestDate = DateTime.UtcNow
            };

            await _unitOfWork.BrandOwnerRequests.AddAsync(request);
            await _unitOfWork.SaveAsync();

            return await GetRequestByIdAsync(request.RequestId);
        }

        public async Task<BrandOwnerRequestDto> GetRequestByIdAsync(int requestId)
        {
            var request = await _unitOfWork.BrandOwnerRequests
                .GetQueryable()
                .AsNoTracking()
                .Include(r => r.User)
                .Include(r => r.Reviewer)
                .FirstOrDefaultAsync(r => r.RequestId == requestId);

            if (request == null)
                throw new KeyNotFoundException("Request not found");

            return MapToDto(request);
        }

        public async Task<IEnumerable<BrandOwnerRequestDto>> GetAllRequestsAsync()
        {
            var requests = await _unitOfWork.BrandOwnerRequests
                .GetQueryable()
                .Include(r => r.User)
                .Include(r => r.Reviewer)
                .OrderByDescending(r => r.RequestDate)
                .ToListAsync();

            return requests.Select(MapToDto);
        }

        public async Task<IEnumerable<BrandOwnerRequestDto>> GetUserRequestsAsync(int userId)
        {
            var requests = await _unitOfWork.BrandOwnerRequests
                .GetQueryable()
                .Include(r => r.User)
                .Include(r => r.Reviewer)
                .Where(r => r.UserId == userId)
                .OrderByDescending(r => r.RequestDate)
                .ToListAsync();

            return requests.Select(MapToDto);
        }

        public async Task<IEnumerable<BrandOwnerRequestDto>> GetPendingRequestsAsync()
        {
            var requests = await _unitOfWork.BrandOwnerRequests
                .GetQueryable()
                .Include(r => r.User)
                .Where(r => r.RequestStatus == RequestStatus.Pending)
                .OrderBy(r => r.RequestDate)
                .ToListAsync();

            return requests.Select(MapToDto);
        }

        public async Task<BrandOwnerRequestDto> ApproveRequestAsync(int requestId, int adminId)
        {
            var request = await _unitOfWork.BrandOwnerRequests
                .GetQueryable()
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.RequestId == requestId);

            if (request == null)
                throw new KeyNotFoundException("Request not found");

            if (request.RequestStatus != RequestStatus.Pending)
                throw new InvalidOperationException("Only pending requests can be approved");

            // ── 1. Update Status ──
            request.RequestStatus = RequestStatus.Approved;
            request.ReviewedBy = adminId;
            request.ReviewDate = DateTime.UtcNow;

            if (request.User != null)
                request.User.UserType = UserType.BrandOwner;

            // ── 2. إنشاء البراند ──
            var brand = new Brand
            {
                UserId = request.UserId,
                BrandName = request.BrandName,
                Description = request.BrandDescription,
                LogoUrl = request.BrandLogoUrl,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            await _unitOfWork.Brands.AddAsync(brand);
            _unitOfWork.BrandOwnerRequests.Update(request);
            await _unitOfWork.SaveAsync();

            // ── 3. Send Email ──
            try
            {
                if (_emailSender is SmartEmailSender smart)
                    smart.SetReceiverId(request.UserId);

                var emailBody = BuildApprovalEmailBody(
                    request.User?.FullName ?? request.User?.UserName,
                    request.BusinessName);

                await _emailSender.SendEmailAsync(
                    request.User?.Email ?? "",
                    "🎉 Your Brand Owner Request Has Been Approved",
                    emailBody);
            }
            catch { /* Email failure doesn't block approval */ }

            // ── 4. Send Notification ──
            await _notificationHelper.SendAsync(
                request.UserId,
                "✅ Request Approved",
                $"Your request to become a Brand Owner for '{request.BusinessName}' has been approved!"
            );

            return await GetRequestByIdAsync(requestId);
        }

        public async Task<BrandOwnerRequestDto> RejectRequestAsync(int requestId, int adminId)
        {
            var request = await _unitOfWork.BrandOwnerRequests
                .GetQueryable()
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.RequestId == requestId);

            if (request == null)
                throw new KeyNotFoundException("Request not found");

            if (request.RequestStatus != RequestStatus.Pending)
                throw new InvalidOperationException("Only pending requests can be rejected");

            // ── 1. Update Status ──
            request.RequestStatus = RequestStatus.Rejected;
            request.ReviewedBy = adminId;
            request.ReviewDate = DateTime.UtcNow;

            _unitOfWork.BrandOwnerRequests.Update(request);
            await _unitOfWork.SaveAsync();

            // ── 2. Send Email ──
            try
            {
                if (_emailSender is SmartEmailSender smart)
                    smart.SetReceiverId(request.UserId);

                var emailBody = BuildRejectionEmailBody(
                    request.User?.FullName ?? request.User?.UserName,
                    request.BusinessName);

                await _emailSender.SendEmailAsync(
                    request.User?.Email ?? "",
                    "Update on Your Brand Owner Request",
                    emailBody);
            }
            catch { /* Email failure doesn't block rejection */ }

            // ── 3. Send Notification ──
            await _notificationHelper.SendAsync(
                request.UserId,
                "❌ Request Rejected",
                $"Your request to become a Brand Owner for '{request.BusinessName}' has been rejected."
            );

            return await GetRequestByIdAsync(requestId);
        }

        public async Task<bool> DeleteRequestAsync(int requestId)
        {
            var request = await _unitOfWork.BrandOwnerRequests.GetByIdAsync(requestId);
            if (request == null)
                return false;

            _unitOfWork.BrandOwnerRequests.Delete(request);
            await _unitOfWork.SaveAsync();

            return true;
        }

        public async Task<bool> HasPendingRequestAsync(int userId)
        {
            return await _unitOfWork.BrandOwnerRequests
                .AnyAsync(r => r.UserId == userId && r.RequestStatus == RequestStatus.Pending);
        }

        // ── Email Templates ──
        private string BuildApprovalEmailBody(string userName, string businessName) => $@"
<h2>🎉 Congratulations, {userName}!</h2>
<p>Your request to become a <strong>Brand Owner</strong> has been <strong>approved</strong>.</p>
<p><strong>Business Name:</strong> {businessName}</p>
<p>You can now start adding your products and managing your brand.</p>
<p>Welcome to our seller community! 🚀</p>";

        private string BuildRejectionEmailBody(string userName, string businessName) => $@"
<h2>Dear {userName},</h2>
<p>We regret to inform you that your request to become a <strong>Brand Owner</strong> has not been approved.</p>
<p><strong>Business Name:</strong> {businessName}</p>
<p>You are welcome to submit a new request with updated information.</p>";

        private BrandOwnerRequestDto MapToDto(BrandOwnerRequest request)
        {
            return new BrandOwnerRequestDto
            {
                RequestId = request.RequestId,
                UserId = request.UserId,
                UserName = request.User?.UserName,
                UserEmail = request.User?.Email,
                BusinessName = request.BusinessName,
                BusinessLicense = request.BusinessLicense,
                BrandName = request.BrandName,
                BrandDescription = request.BrandDescription,
                BrandLogo = request.BrandLogoUrl,
                RequestStatus = request.RequestStatus,
                RequestStatusText = GetStatusText(request.RequestStatus),
                RequestDate = request.RequestDate,
                ReviewedBy = request.ReviewedBy,
                ReviewerName = request.Reviewer?.UserName,
                ReviewDate = request.ReviewDate
            };
        }

        private string GetStatusText(RequestStatus status) => status switch
        {
            RequestStatus.Pending => "Pending",
            RequestStatus.Approved => "Approved",
            RequestStatus.Rejected => "Rejected",
            _ => "Unknown"
        };
    }
}