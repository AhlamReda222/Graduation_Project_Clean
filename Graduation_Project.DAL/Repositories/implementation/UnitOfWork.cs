using Graduation_Project.DAL.DataBase;
using Graduation_Project.DAL.Models.Entities;
using Graduation_Project.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;
namespace Graduation_Project.DAL.Repositories.Implementations
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        public IGenericRepository<RefreshToken> RefreshTokens { get; }
        public IGenericRepository<ApplicationUser> ApplicationUsers { get; }
        public IGenericRepository<Brand> Brands { get; }
        public IGenericRepository<Category> Categories { get; }
        public IGenericRepository<BrandOwnerRequest> BrandOwnerRequests { get; }
        public IGenericRepository<Product> Products { get; }
        public IGenericRepository<CartItem> CartItems { get; }
        public IGenericRepository<Order> Orders { get; }
        public IGenericRepository<OrderItem> OrderItems { get; }
        public IGenericRepository<Review> Reviews { get; }
        public IGenericRepository<Discount> Discounts { get; }

        public IGenericRepository<Conversation> Conversations { get; }
        public IGenericRepository<Message> Messages { get; }
        public IGenericRepository<Profile> Profiles { get; }
        
        public IGenericRepository<Notification> Notifications { get; }
        public IGenericRepository<InAppEmail> InAppEmails { get; }
        public IGenericRepository<PasswordResetCode> PasswordResetCodes { get; }

        public IGenericRepository<ProductVariant> ProductVariants { get; }
        public  IGenericRepository<ProductCustomizationZone> ProductCustomizationZones { get; }
        public IGenericRepository<PrintingTechnique> PrintingTechniques { get; }
        public IGenericRepository<OrderItemCustomization> OrderItemCustomizations { get; }
        public IGenericRepository<WishlistItem> WishlistItems { get; }

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
            RefreshTokens = new GenericRepository<RefreshToken>(_context);
            Brands = new GenericRepository<Brand>(_context);
            Categories = new GenericRepository<Category>(_context);
            BrandOwnerRequests = new GenericRepository<BrandOwnerRequest>(_context);
            Products = new GenericRepository<Product>(_context);
            CartItems = new GenericRepository<CartItem>(_context);
            Orders = new GenericRepository<Order>(_context);
            OrderItems = new GenericRepository<OrderItem>(_context);
            Reviews = new GenericRepository<Review>(_context);
            Discounts = new GenericRepository<Discount>(_context);
            Conversations = new GenericRepository<Conversation>(_context);
            Messages = new GenericRepository<Message>(_context);
            Profiles = new GenericRepository<Profile>(_context);
            ApplicationUsers = new GenericRepository<ApplicationUser>(_context);
            ProductVariants = new GenericRepository<ProductVariant>(_context);
            ProductCustomizationZones = new GenericRepository<ProductCustomizationZone>(_context);
            PrintingTechniques = new GenericRepository<PrintingTechnique>(_context);
            OrderItemCustomizations = new GenericRepository<OrderItemCustomization>(_context);
               Notifications = new GenericRepository<Notification>(_context);
            InAppEmails = new GenericRepository<InAppEmail>(_context);
            PasswordResetCodes = new GenericRepository<PasswordResetCode>(_context);
             WishlistItems = new GenericRepository<WishlistItem>(_context);

        }

        public async Task<int> SaveAsync()
            => await _context.SaveChangesAsync();

            public async Task<IDbContextTransaction> BeginTransactionAsync()
{
    return await _context.Database.BeginTransactionAsync();
}
    }
}