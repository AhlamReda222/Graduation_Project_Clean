using Graduation_Project.BLL.Common;
using Graduation_Project.BLL.DTOs.Wishlist;
using Graduation_Project.BLL.Services.Interfaces;
using Graduation_Project.DAL.Models.Entities;
using Graduation_Project.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Graduation_Project.BLL.Services.Implementations
{
    public class WishlistService : IWishlistService
    {
        private readonly IUnitOfWork _unitOfWork;

        public WishlistService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // ── جلب كل الـ Wishlist ──
        public async Task<ServiceResult<IEnumerable<WishlistItemDto>>> GetMyWishlistAsync(int userId)
        {
            try
            {
                var items = await _unitOfWork.WishlistItems
                    .GetQueryable()
                    .Include(w => w.Product)
                        .ThenInclude(p => p.Brand)
                    .Include(w => w.Product)
                        .ThenInclude(p => p.Variants)
                    .Where(w => w.UserId == userId)
                    .OrderByDescending(w => w.AddedAt)
                    .ToListAsync();

                return ServiceResult<IEnumerable<WishlistItemDto>>.Success(
                    items.Select(MapToDto));
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<WishlistItemDto>>.Failure($"Error: {ex.Message}");
            }
        }

        // ── إضافة منتج للـ Wishlist ──
        public async Task<ServiceResult<WishlistItemDto>> AddToWishlistAsync(int userId, int productId)
        {
            try
            {
                // تأكد إن المنتج موجود
                var product = await _unitOfWork.Products
                    .GetQueryable()
                    .Include(p => p.Brand)
                    .Include(p => p.Variants)
                    .FirstOrDefaultAsync(p => p.ProductId == productId);

                if (product == null)
                    return ServiceResult<WishlistItemDto>.Failure("Product not found.");

                // تأكد إنه مش موجود بالفعل
                var existing = await _unitOfWork.WishlistItems
                    .FindOneAsync(w => w.UserId == userId && w.ProductId == productId);

                if (existing != null)
                    return ServiceResult<WishlistItemDto>.Failure("Product already in wishlist.");

                var wishlistItem = new WishlistItem
                {
                    UserId = userId,
                    ProductId = productId,
                    AddedAt = DateTime.UtcNow
                };

                await _unitOfWork.WishlistItems.AddAsync(wishlistItem);
                await _unitOfWork.SaveAsync();

                // جيب الـ item مع الـ navigation properties
                wishlistItem.Product = product;

                return ServiceResult<WishlistItemDto>.Success(MapToDto(wishlistItem));
            }
            catch (Exception ex)
            {
                return ServiceResult<WishlistItemDto>.Failure($"Error: {ex.Message}");
            }
        }

        // ── إزالة منتج من الـ Wishlist ──
        public async Task<ServiceResult<bool>> RemoveFromWishlistAsync(int userId, int productId)
        {
            try
            {
                var item = await _unitOfWork.WishlistItems
                    .FindOneAsync(w => w.UserId == userId && w.ProductId == productId);

                if (item == null)
                    return ServiceResult<bool>.Failure("Item not found in wishlist.");

                _unitOfWork.WishlistItems.Delete(item);
                await _unitOfWork.SaveAsync();

                return ServiceResult<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return ServiceResult<bool>.Failure($"Error: {ex.Message}");
            }
        }

        // ── تأكد إن المنتج في الـ Wishlist ──
        public async Task<ServiceResult<bool>> IsInWishlistAsync(int userId, int productId)
        {
            try
            {
                var exists = await _unitOfWork.WishlistItems
                    .AnyAsync(w => w.UserId == userId && w.ProductId == productId);

                return ServiceResult<bool>.Success(exists);
            }
            catch (Exception ex)
            {
                return ServiceResult<bool>.Failure($"Error: {ex.Message}");
            }
        }

        // ── مسح كل الـ Wishlist ──
        public async Task<ServiceResult<bool>> ClearWishlistAsync(int userId)
        {
            try
            {
                var items = await _unitOfWork.WishlistItems
                    .FindAsync(w => w.UserId == userId);

                foreach (var item in items)
                    _unitOfWork.WishlistItems.Delete(item);

                await _unitOfWork.SaveAsync();

                return ServiceResult<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return ServiceResult<bool>.Failure($"Error: {ex.Message}");
            }
        }

        private WishlistItemDto MapToDto(WishlistItem w) => new WishlistItemDto
        {
            WishlistItemId = w.WishlistItemId,
            ProductId = w.ProductId,
            ProductName = w.Product?.ProductName,
            ProductImage = w.Product?.ImageUrls?.Split(',').FirstOrDefault()?.Trim(),
            BrandName = w.Product?.Brand?.BrandName,
            Price = w.Product?.Variants?.Any() == true
                ? w.Product.Variants.Min(v => v.Price)
                : w.Product?.BasePrice ?? 0,
            IsAvailable = w.Product?.IsActive ?? false,
            AddedAt = w.AddedAt
        };
    }
}