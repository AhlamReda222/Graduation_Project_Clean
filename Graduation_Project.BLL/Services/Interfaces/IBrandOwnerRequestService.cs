using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Graduation_Project.BLL.DTOs.BrandOwnerRequest;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Graduation_Project.BLL.Services.Interfaces
{
    public interface IBrandOwnerRequestService
    {
        Task<BrandOwnerRequestDto> CreateRequestAsync(int userId, CreateBrandOwnerRequestDto dto);
        Task<BrandOwnerRequestDto> GetRequestByIdAsync(int requestId);
        Task<IEnumerable<BrandOwnerRequestDto>> GetAllRequestsAsync();
        Task<IEnumerable<BrandOwnerRequestDto>> GetUserRequestsAsync(int userId);
        Task<IEnumerable<BrandOwnerRequestDto>> GetPendingRequestsAsync();
        Task<BrandOwnerRequestDto> ApproveRequestAsync(int requestId, int adminId);
        Task<BrandOwnerRequestDto> RejectRequestAsync(int requestId, int adminId);
        Task<bool> DeleteRequestAsync(int requestId);
        Task<bool> HasPendingRequestAsync(int userId);
    }
}