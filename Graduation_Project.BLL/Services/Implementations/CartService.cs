
using Graduation_Project.BLL.Common;
using Graduation_Project.BLL.DTOs.Cart;
using Graduation_Project.BLL.Services.Interfaces;
using Graduation_Project.DAL.Models.Entities;
using Graduation_Project.DAL.Models.Enums;
using Graduation_Project.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Graduation_Project.BLL.Services.Implementations
{
    public class CartService : ICartService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CartService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // ============================================================
        // ADD TO CART
        // ============================================================
 // ============================================================
        // ADD TO CART
        // ============================================================
        public async Task<ServiceResult<CartItemDto>> AddToCartAsync(int userId, AddToCartDto dto)
        {
            try
            {
                var product = await _unitOfWork.Products
                    .GetQueryable()
                    .Include(p => p.Brand)
                    .Include(p => p.CustomizationZones)
                    .Include(p => p.Variants)
                    .FirstOrDefaultAsync(p => p.ProductId == dto.ProductId
                                           && p.IsActive
                                           && p.ApprovalStatus == ApprovalStatus.Approved);

                if (product == null)
                    return ServiceResult<CartItemDto>.Failure("Product not found or not available");

                ProductVariant? variant = null;

                // ================= VARIANT =================
                if (product.Variants != null && product.Variants.Any())
                {
                    if (dto.VariantId == null)
                        return ServiceResult<CartItemDto>.Failure("Variant required");

                    variant = product.Variants
                        .FirstOrDefault(v => v.VariantId == dto.VariantId.Value);

                    if (variant == null)
                        return ServiceResult<CartItemDto>.Failure("Selected variant not found");

                    if (variant.StockQuantity < dto.Quantity)
                        return ServiceResult<CartItemDto>.Failure("Not enough stock");
                }
                else
                {
                    if (dto.VariantId != null)
                        return ServiceResult<CartItemDto>.Failure("Product has no variants");

                    if (product.StockQuantity < dto.Quantity)
                        return ServiceResult<CartItemDto>.Failure("Not enough stock");
                }

                // ================= CUSTOMIZATION =================
                decimal customizationPrice = 0;
                PrintingTechnique? technique = null;

                if (dto.Customization != null)
                {
                    if (!product.AllowsCustomization)
                        return ServiceResult<CartItemDto>.Failure("Customization not allowed");

                    var zoneExists = product.CustomizationZones?
                        .Any(z => z.Zone == dto.Customization.Zone && z.IsAvailable) ?? false;

                    if (!zoneExists)
                        return ServiceResult<CartItemDto>.Failure("Invalid zone");

                    if (!string.IsNullOrEmpty(dto.Customization.DesignImageUrl) && !product.AllowsPrinting)
                        return ServiceResult<CartItemDto>.Failure("Printing not allowed");

                    if (!string.IsNullOrEmpty(dto.Customization.DesignText) && !product.AllowsText)
                        return ServiceResult<CartItemDto>.Failure("Text not allowed");

                    technique = await _unitOfWork.PrintingTechniques
                        .GetQueryable()
                        .FirstOrDefaultAsync(t => t.TechniqueId == dto.Customization.TechniqueId && t.IsActive);

                    if (technique == null)
                        return ServiceResult<CartItemDto>.Failure("Technique not found");

                    customizationPrice = technique.Price;
                }

                // ================= SIGNATURE =================
                var signature = BuildSignature(dto);

                var existingItem = await _unitOfWork.CartItems
                    .GetQueryable()
                    .FirstOrDefaultAsync(c => c.UserId == userId && c.Signature == signature);

                if (existingItem != null)
                {
                    var newQty = existingItem.Quantity + dto.Quantity;

                    if (variant != null && variant.StockQuantity < newQty)
                        return ServiceResult<CartItemDto>.Failure("Not enough stock");

                    if (variant == null && product.StockQuantity < newQty)
                        return ServiceResult<CartItemDto>.Failure("Not enough stock");

                    existingItem.Quantity = newQty;
                    _unitOfWork.CartItems.Update(existingItem);
                }
                else
                {
                    existingItem = new CartItem
                    {
                        UserId = userId,
                        ProductId = dto.ProductId,
                        VariantId = dto.VariantId,
                        Quantity = dto.Quantity,
                        AddedAt = DateTime.UtcNow,
                        CustomizationZone = dto.Customization?.Zone,
                        TechniqueId = dto.Customization?.TechniqueId,
                        DesignImageUrl = dto.Customization?.DesignImageUrl,
                        DesignText = dto.Customization?.DesignText,
                        Signature = signature
                    };

                    await _unitOfWork.CartItems.AddAsync(existingItem);
                }

                await _unitOfWork.SaveAsync();

                return ServiceResult<CartItemDto>.Success(
                    MapToDto(existingItem, product, variant, technique),
                    "Added successfully"
                );
            }
          catch (Exception ex)
{
    return ServiceResult<CartItemDto>.Failure(
        $"Error: {ex.Message} | Inner: {ex.InnerException?.Message}"
    );
}
        }

        // ============================================================
        // GET CART
        // ============================================================
        public async Task<ServiceResult<CartSummaryDto>> GetCartAsync(int userId)
        {
            try
            {
                var items = await _unitOfWork.CartItems
                    .GetQueryable()
                    .Include(c => c.Product).ThenInclude(p => p.Brand)
                    .Include(c => c.ProductVariant)
                    .Include(c => c.Technique)
                    .Where(c => c.UserId == userId)
                    .ToListAsync();

                var itemDtos = items.Select(item =>
                    MapToDto(item, item.Product, item.ProductVariant, item.Technique)
                ).ToList();

                var subTotal = itemDtos.Sum(i => i.UnitPrice * i.Quantity);
                var custTotal = itemDtos.Sum(i => i.CustomizationPrice * i.Quantity);

                return ServiceResult<CartSummaryDto>.Success(new CartSummaryDto
                {
                    Items = itemDtos,
                    TotalItems = itemDtos.Sum(i => i.Quantity),
                    SubTotal = subTotal,
                    CustomizationTotal = custTotal,
                    Total = subTotal + custTotal
                });
            }
            catch (Exception ex)
            {
                return ServiceResult<CartSummaryDto>.Failure(ex.Message);
            }
        }

        // ============================================================
        // UPDATE QUANTITY
        // ============================================================
        public async Task<ServiceResult<CartItemDto>> UpdateQuantityAsync(int userId, int cartItemId, int quantity)
        {
            try
            {
                var item = await _unitOfWork.CartItems
                    .GetQueryable()
                    .Include(c => c.Product).ThenInclude(p => p.Brand)
                    .Include(c => c.ProductVariant)
                    .Include(c => c.Technique)
                    .FirstOrDefaultAsync(c => c.CartItemId == cartItemId && c.UserId == userId);

                if (item == null)
                    return ServiceResult<CartItemDto>.Failure("Cart item not found");

                // ✅ التحقق من الـ Stock
                if (item.ProductVariant != null)
                {
                    // منتج بـ variant
                    if (item.ProductVariant.StockQuantity < quantity)
                        return ServiceResult<CartItemDto>.Failure($"Only {item.ProductVariant.StockQuantity} items available");
                }
                else
                {
                    // منتج بدون variant
                    if (item.Product.StockQuantity < quantity)
                        return ServiceResult<CartItemDto>.Failure($"Only {item.Product.StockQuantity} items available");
                }

                item.Quantity = quantity;
                _unitOfWork.CartItems.Update(item);
                await _unitOfWork.SaveAsync();

                return ServiceResult<CartItemDto>.Success(
                    MapToDto(item, item.Product, item.ProductVariant, item.Technique)
                );
            }
            catch (Exception ex)
            {
                return ServiceResult<CartItemDto>.Failure($"Error updating cart: {ex.Message}");
            }
        }

        // ============================================================
        // REMOVE FROM CART
        // ============================================================
        public async Task<ServiceResult<bool>> RemoveFromCartAsync(int userId, int cartItemId)
        {
            try
            {
                var item = await _unitOfWork.CartItems
                    .GetQueryable()
                    .FirstOrDefaultAsync(c => c.CartItemId == cartItemId && c.UserId == userId);

                if (item == null)
                    return ServiceResult<bool>.Failure("Cart item not found");

                _unitOfWork.CartItems.Delete(item);
                await _unitOfWork.SaveAsync();

                return ServiceResult<bool>.Success(true, "Item removed from cart");
            }
            catch (Exception ex)
            {
                return ServiceResult<bool>.Failure($"Error removing from cart: {ex.Message}");
            }
        }

        // ============================================================
        // CLEAR CART
        // ============================================================
        public async Task<ServiceResult<bool>> ClearCartAsync(int userId)
        {
            try
            {
                var items = await _unitOfWork.CartItems
                    .FindAsync(c => c.UserId == userId);

                if (!items.Any())
                    return ServiceResult<bool>.Success(true, "Cart is already empty");

                foreach (var item in items)
                    _unitOfWork.CartItems.Delete(item);

                await _unitOfWork.SaveAsync();

                return ServiceResult<bool>.Success(true, "Cart cleared successfully");
            }
            catch (Exception ex)
            {
                return ServiceResult<bool>.Failure($"Error clearing cart: {ex.Message}");
            }
        }

        // ============================================================
        // PRIVATE HELPERS
        // ============================================================

  // ============================================================
        // MAP DTO
        // ============================================================
        private CartItemDto MapToDto(
            CartItem item,
            Product product,
            ProductVariant? variant,
            PrintingTechnique? technique)
        {
            var unitPrice = variant?.Price ?? product.BasePrice;
            var custPrice = technique?.Price ?? 0;

            return new CartItemDto
            {
                CartItemId = item.CartItemId,
                ProductId = item.ProductId,
                ProductName = product.ProductName,
                ProductImage = product.ImageUrls?.Split(',').FirstOrDefault()?.Trim(),
                BrandName = product.Brand?.BrandName,

                VariantId = item.VariantId,
                Size = variant?.Size,
                Color = variant?.Color,

                UnitPrice = unitPrice,
                Quantity = item.Quantity,

                HasCustomization = item.TechniqueId != null,
                CustomizationPrice = custPrice,

                Subtotal =
                    (unitPrice * item.Quantity) +
                    (item.TechniqueId != null ? custPrice * item.Quantity : 0),

                Customization = item.TechniqueId != null
                    ? new CartItemCustomizationDto
                    {
                        Zone = item.CustomizationZone.ToString(),
                        TechniqueName = technique?.Name,
                        DesignImageUrl = item.DesignImageUrl,
                        DesignText = item.DesignText,
                        TechniquePrice = custPrice
                    }
                    : null
            };
        }

        // ============================================================
        // SIGNATURE (FIXED)
        // ============================================================
        private string BuildSignature(AddToCartDto dto)
        {
            var c = dto.Customization;

            return $"{dto.ProductId}|" +
                   $"{dto.VariantId ?? 0}|" +
                   $"{c?.TechniqueId ?? 0}|" +
                   $"{c?.Zone ?? 0}|" +
                   $"{(c?.DesignText ?? "").Trim().ToLower()}|" +
                   $"{(c?.DesignImageUrl ?? "").Trim()}";
        }
    }}