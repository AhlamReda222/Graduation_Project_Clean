using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Graduation_Project.BLL.Common;
using Graduation_Project.BLL.DTOs.Wishlist;
using Graduation_Project.DAL.Models.Entities;
namespace Graduation_Project.BLL.Services.Interfaces
{
    public interface IWishlistService
    {
        Task<ServiceResult<IEnumerable<WishlistItemDto>>> GetMyWishlistAsync(int userId);
        Task<ServiceResult<WishlistItemDto>> AddToWishlistAsync(int userId, int productId);
        Task<ServiceResult<bool>> RemoveFromWishlistAsync(int userId, int productId);
        Task<ServiceResult<bool>> IsInWishlistAsync(int userId, int productId);
        Task<ServiceResult<bool>> ClearWishlistAsync(int userId);
    }
}