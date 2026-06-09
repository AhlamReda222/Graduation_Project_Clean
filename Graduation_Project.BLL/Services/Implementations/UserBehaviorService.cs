// FILE: BLL/Services/Implementations/UserBehaviorService.cs
using Graduation_Project.BLL.Common;
using Graduation_Project.BLL.DTOs.Tracking;
using Graduation_Project.BLL.Services.Interfaces;
using Graduation_Project.DAL.DataBase;
using Graduation_Project.DAL.Enums;
using Graduation_Project.DAL.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace Graduation_Project.BLL.Services.Implementations
{
    public class UserBehaviorService : IUserBehaviorService
    {
        private readonly ApplicationDbContext _context;

        public UserBehaviorService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ServiceResult<bool>> TrackEventAsync(
            TrackUserBehaviorRequest request,
            int? userId)
        {
            if (request == null)
                return ServiceResult<bool>.Failure("Request body is required.");

            if (string.IsNullOrWhiteSpace(request.SessionId))
                return ServiceResult<bool>.Failure("SessionId is required.");

            if (string.IsNullOrWhiteSpace(request.ActionType))
                return ServiceResult<bool>.Failure("ActionType is required.");

            if (!TryMapActionType(request.ActionType, out var eventType))
                return ServiceResult<bool>.Failure(
                    "Invalid ActionType. Allowed: view, click, cart, wishlist, purchase, search.");

            if (eventType == UserBehaviorEventType.Search &&
                string.IsNullOrWhiteSpace(request.SearchQuery))
                return ServiceResult<bool>.Failure("SearchQuery is required for search action.");

            if (RequiresProductId(eventType) && request.ProductId == null)
                return ServiceResult<bool>.Failure("ProductId is required for this action type.");

            var idsValidation = await ValidateRelatedIdsAsync(request);
            if (!idsValidation.Succeeded)
                return idsValidation;

            var behaviorEvent = new UserBehaviorEvent
            {
                UserId      = userId,
                SessionId   = request.SessionId.Trim(),
                EventType   = eventType,
                ProductId   = request.ProductId,
                CategoryId  = request.CategoryId,
                BrandId     = request.BrandId,
                SearchQuery = string.IsNullOrWhiteSpace(request.SearchQuery)
                    ? null : request.SearchQuery.Trim(),
                SourcePage  = string.IsNullOrWhiteSpace(request.SourcePage)
                    ? null : request.SourcePage.Trim(),
                CreatedAt   = DateTime.UtcNow
            };

            await _context.UserBehaviorEvents.AddAsync(behaviorEvent);
            await _context.SaveChangesAsync();

            return ServiceResult<bool>.Success(true, "Event tracked successfully.");
        }

        // ✅ بعد Login - اربط الـ events الـ anonymous بالـ User
        public async Task LinkSessionToUserAsync(string sessionId, int userId)
        {
            var anonymousEvents = await _context.UserBehaviorEvents
                .Where(e => e.SessionId == sessionId && e.UserId == null)
                .ToListAsync();

            if (!anonymousEvents.Any()) return;

            foreach (var e in anonymousEvents)
                e.UserId = userId;

            await _context.SaveChangesAsync();
        }

        // ── private helpers ──────────────────────────────────

        private async Task<ServiceResult<bool>> ValidateRelatedIdsAsync(
            TrackUserBehaviorRequest request)
        {
            if (request.ProductId.HasValue)
            {
                var exists = await _context.Products
                    .AnyAsync(p => p.ProductId == request.ProductId.Value && p.IsActive);

                if (!exists)
                    return ServiceResult<bool>.Failure(
                        $"Product {request.ProductId.Value} does not exist or is not active.");
            }

            if (request.CategoryId.HasValue)
            {
                var exists = await _context.Categories
                    .AnyAsync(c => c.CategoryId == request.CategoryId.Value);

                if (!exists)
                    return ServiceResult<bool>.Failure(
                        $"Category {request.CategoryId.Value} does not exist.");
            }

            if (request.BrandId.HasValue)
            {
                var exists = await _context.Brands
                    .AnyAsync(b => b.BrandId == request.BrandId.Value && b.IsActive);

                if (!exists)
                    return ServiceResult<bool>.Failure(
                        $"Brand {request.BrandId.Value} does not exist or is not active.");
            }

            return ServiceResult<bool>.Success(true);
        }

        private static bool RequiresProductId(UserBehaviorEventType eventType) =>
            eventType is UserBehaviorEventType.View
                      or UserBehaviorEventType.Click
                      or UserBehaviorEventType.Cart
                      or UserBehaviorEventType.Wishlist
                      or UserBehaviorEventType.Purchase;

        private static bool TryMapActionType(
            string actionType,
            out UserBehaviorEventType eventType)
        {
            eventType = default;
            if (string.IsNullOrWhiteSpace(actionType)) return false;

            eventType = actionType.Trim().ToLower() switch
            {
                "view"                          => UserBehaviorEventType.View,
                "click"                         => UserBehaviorEventType.Click,
                "cart" or "add_to_cart"
                    or "addtocart"              => UserBehaviorEventType.Cart,
                "wishlist" or "favorite"
                    or "favourite"              => UserBehaviorEventType.Wishlist,
                "purchase" or "buy" or "order"  => UserBehaviorEventType.Purchase,
                "search"                        => UserBehaviorEventType.Search,
                _                               => (UserBehaviorEventType)(-1)
            };

            return (int)eventType != -1;
        }
    }
}