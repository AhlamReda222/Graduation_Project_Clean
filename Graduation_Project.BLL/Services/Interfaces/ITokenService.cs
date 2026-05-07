using Graduation_Project.DAL.Models.Entities;

namespace Graduation_Project.BLL.Services.Interfaces
{
    public interface ITokenService
    {
        string GenerateJwtToken(ApplicationUser user);
        string GenerateRefreshToken();
    }
}
