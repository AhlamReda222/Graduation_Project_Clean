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

        public async Task<ServiceResult<bool>> TrackEventAsync(TrackUserBehaviorRequest request, int? userId)
        {
            if (request == null)
                return ServiceResult<bool>.Failure("Request body is required.");

            if (string.IsNullOrWhiteSpace(request.SessionId))
                return ServiceResult<bool>.Failure("SessionId is required.");

            if (string.IsNullOrWhiteSpace(request.ActionType))
                return ServiceResult<bool>.Failure("ActionType is required.");

            if (!TryMapActionType(request.ActionType, out var eventType))
            {
                return ServiceResult<bool>.Failure(
                    "Invalid ActionType. Allowed values: view, click, cart, wishlist, purchase, search."
                );
            }

            if (eventType == UserBehaviorEventType.Search &&
                string.IsNullOrWhiteSpace(request.SearchQuery))
            {
                return ServiceResult<bool>.Failure("SearchQuery is required for search action.");
            }

            if (RequiresProductId(eventType) && request.ProductId == null)
            {
                return ServiceResult<bool>.Failure("ProductId is required for this action type.");
            }

            var idsValidationResult = await ValidateRelatedIdsAsync(request);

            if (!idsValidationResult.Succeeded)
                return idsValidationResult;

            var behaviorEvent = new UserBehaviorEvent
            {
                UserId = userId,
                SessionId = request.SessionId.Trim(),
                EventType = eventType,
                ProductId = request.ProductId,
                CategoryId = request.CategoryId,
                BrandId = request.BrandId,
                SearchQuery = string.IsNullOrWhiteSpace(request.SearchQuery)
                    ? null
                    : request.SearchQuery.Trim(),
                SourcePage = string.IsNullOrWhiteSpace(request.SourcePage)
                    ? null
                    : request.SourcePage.Trim(),
                CreatedAt = DateTime.UtcNow
            };

            await _context.UserBehaviorEvents.AddAsync(behaviorEvent);
            await _context.SaveChangesAsync();

            return ServiceResult<bool>.Success(true, "Event tracked successfully.");
        }

        private async Task<ServiceResult<bool>> ValidateRelatedIdsAsync(TrackUserBehaviorRequest request)
        {
            if (request.ProductId.HasValue)
            {
                var productExists = await _context.Products
                    .AnyAsync(p =>
                        p.ProductId == request.ProductId.Value &&
                        p.IsActive);

                if (!productExists)
                    return ServiceResult<bool>.Failure(
                        $"Product with id {request.ProductId.Value} does not exist or is not active."
                    );
            }

            if (request.CategoryId.HasValue)
            {
                var categoryExists = await _context.Categories
                    .AnyAsync(c => c.CategoryId == request.CategoryId.Value);

                if (!categoryExists)
                    return ServiceResult<bool>.Failure(
                        $"Category with id {request.CategoryId.Value} does not exist."
                    );
            }

            if (request.BrandId.HasValue)
            {
                var brandExists = await _context.Brands
                    .AnyAsync(b =>
                        b.BrandId == request.BrandId.Value &&
                        b.IsActive);

                if (!brandExists)
                    return ServiceResult<bool>.Failure(
                        $"Brand with id {request.BrandId.Value} does not exist or is not active."
                    );
            }

            return ServiceResult<bool>.Success(true);
        }

        private static bool RequiresProductId(UserBehaviorEventType eventType)
        {
            return eventType == UserBehaviorEventType.View
                || eventType == UserBehaviorEventType.Click
                || eventType == UserBehaviorEventType.Cart
                || eventType == UserBehaviorEventType.Wishlist
                || eventType == UserBehaviorEventType.Purchase;
        }

        private static bool TryMapActionType(string actionType, out UserBehaviorEventType eventType)
        {
            eventType = default;

            if (string.IsNullOrWhiteSpace(actionType))
                return false;

            var normalizedAction = actionType.Trim().ToLower();

            switch (normalizedAction)
            {
                case "view":
                    eventType = UserBehaviorEventType.View;
                    return true;

                case "click":
                    eventType = UserBehaviorEventType.Click;
                    return true;

                case "cart":
                case "add_to_cart":
                case "addtocart":
                    eventType = UserBehaviorEventType.Cart;
                    return true;

                case "wishlist":
                case "favorite":
                case "favourite":
                    eventType = UserBehaviorEventType.Wishlist;
                    return true;

                case "purchase":
                case "buy":
                case "order":
                    eventType = UserBehaviorEventType.Purchase;
                    return true;

                case "search":
                    eventType = UserBehaviorEventType.Search;
                    return true;

                default:
                    return false;
            }
        }
    }
}