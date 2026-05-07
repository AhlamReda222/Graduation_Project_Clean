using Graduation_Project.DAL.Models.Entities;
using Microsoft.EntityFrameworkCore.Storage;

namespace Graduation_Project.DAL.Repositories.Interfaces
{
    public interface IUnitOfWork
    {
        IGenericRepository<RefreshToken> RefreshTokens { get; }
        IGenericRepository<Brand> Brands { get; }
        IGenericRepository<Category> Categories { get; }
        IGenericRepository<BrandOwnerRequest> BrandOwnerRequests { get; }
        IGenericRepository<Product> Products { get; }
        IGenericRepository<CartItem> CartItems { get; }
        IGenericRepository<Order> Orders { get; }
        IGenericRepository<OrderItem> OrderItems { get; }
        IGenericRepository<Review> Reviews { get; }
        IGenericRepository<Discount> Discounts { get; }
        IGenericRepository<Conversation> Conversations { get; }
        IGenericRepository<Message> Messages { get; }
        IGenericRepository<Profile> Profiles { get; }
        IGenericRepository<ApplicationUser> ApplicationUsers { get; }
        IGenericRepository<ProductVariant> ProductVariants { get; }
        IGenericRepository<ProductCustomizationZone> ProductCustomizationZones { get; }
        IGenericRepository<PrintingTechnique> PrintingTechniques { get; }
        IGenericRepository<OrderItemCustomization> OrderItemCustomizations { get; }
         IGenericRepository<Notification> Notifications { get; }
        IGenericRepository<InAppEmail> InAppEmails { get; }
        IGenericRepository<PasswordResetCode> PasswordResetCodes { get; }
                IGenericRepository<WishlistItem> WishlistItems { get; }

        Task<IDbContextTransaction> BeginTransactionAsync();
        Task<int> SaveAsync();
    }
}