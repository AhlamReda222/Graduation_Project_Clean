using Graduation_Project.BLL.Common;
using Graduation_Project.BLL.DTOs.Contract;
using Graduation_Project.BLL.Services.Interfaces;
using Graduation_Project.DAL.Models.Enums;
using Graduation_Project.DAL.Repositories.Interfaces;

namespace Graduation_Project.BLL.Services.Implementations
{
    public class ContractService : IContractService
    {
        private readonly IUnitOfWork _unitOfWork;

        // Contract text (English version)
        private const string ContractText = """
            Brand Owner Agreement - Local Brand Platform

            1. Quality Standards
               - All products must be original and meet the stated specifications.
               - Selling counterfeit or illegal products is strictly prohibited.

            2. Pricing Policy
               - Brand owners have full freedom to set product prices.
               - Prices must be transparent and include all applicable fees.

            3. Customer Service
               - Brand owners must respond to customer inquiries within 48 hours.
               - Complaints must be handled professionally and in a timely manner.

            4. Return & Refund Policy
               - A clear return and refund policy must be provided.
               - Refunds must be issued for defective or incorrect products.

            5. Platform Rights
               - The platform reserves the right to suspend any brand that violates these terms.
               - The platform may take a commission from each completed transaction.

            By accepting this agreement, you confirm that you have read, understood, and agree to all the terms stated above.
            """;

        public ContractService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // Get contract text
        public ServiceResult<ContractDto> GetContract()
        {
            return ServiceResult<ContractDto>.Success(new ContractDto
            {
                ContractText = ContractText
            });
        }

        // Brand owner accepts contract
        public async Task<ServiceResult<bool>> AcceptContractAsync(int userId)
        {
            var user = await _unitOfWork.ApplicationUsers.GetByIdAsync(userId);
            if (user == null)
                return ServiceResult<bool>.Failure("User not found");

            // Must be BrandOwner
            if (user.UserType != UserType.BrandOwner)
                return ServiceResult<bool>.Failure("Only brand owners can accept the contract");

            // Already accepted
            if (user.HasAcceptedContract)
                return ServiceResult<bool>.Failure("Contract already accepted");

            user.HasAcceptedContract = true;
            user.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.ApplicationUsers.Update(user);
            await _unitOfWork.SaveAsync();

            return ServiceResult<bool>.Success(true, "Contract accepted successfully. You can now create your brand!");
        }

        // Check contract status
        public async Task<ServiceResult<ContractStatusDto>> GetContractStatusAsync(int userId)
        {
            var user = await _unitOfWork.ApplicationUsers.GetByIdAsync(userId);
            if (user == null)
                return ServiceResult<ContractStatusDto>.Failure("User not found");

            return ServiceResult<ContractStatusDto>.Success(new ContractStatusDto
            {
                HasAcceptedContract = user.HasAcceptedContract,
                Message = user.HasAcceptedContract
                    ? "Contract accepted. You can now create your brand."
                    : "Please accept the contract to continue."
            });
        }
    }
}