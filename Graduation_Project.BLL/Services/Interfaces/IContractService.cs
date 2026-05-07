using Graduation_Project.BLL.Common;
using Graduation_Project.BLL.DTOs.Contract;

namespace Graduation_Project.BLL.Services.Interfaces
{
    public interface IContractService
    {
        ServiceResult<ContractDto> GetContract();
        Task<ServiceResult<bool>> AcceptContractAsync(int userId);
        Task<ServiceResult<ContractStatusDto>> GetContractStatusAsync(int userId);
    }
}