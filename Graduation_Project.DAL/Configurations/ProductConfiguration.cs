using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Graduation_Project.DAL.Models.Entities;
using Graduation_Project.DAL.Models.Enums;

namespace Graduation_Project.DAL.Configurations
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.ToTable("Products");

            builder.HasKey(p => p.ProductId);

            builder.Property(p => p.ProductId)
                .ValueGeneratedOnAdd();

            builder.Property(p => p.BrandId)
                .IsRequired();

            builder.Property(p => p.CategoryId)
                .IsRequired();

            builder.Property(p => p.ProductName)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(p => p.Description)
                .HasColumnType("text");


            builder.Property(p => p.ImageUrls)
                .HasColumnType("text");

            builder.Property(p => p.ApprovalStatus)
                .IsRequired()
                .HasConversion<int>()
                .HasDefaultValue(ApprovalStatus.Pending);

            builder.Property(p => p.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("NOW()");

            builder.Property(p => p.UpdatedAt)
                .IsRequired()
                .HasDefaultValueSql("NOW()");

            builder.Property(p => p.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            builder.Property(p => p.AverageRating)
                .HasColumnType("decimal(3,2)")
                .HasDefaultValue(0.00m);

            builder.Property(p => p.ReviewCount)
                .IsRequired()
                .HasDefaultValue(0);

            builder.HasIndex(p => p.ProductName)
                .HasDatabaseName("IX_Products_ProductName");

            builder.HasIndex(p => p.BrandId)
                .HasDatabaseName("IX_Products_BrandId");

            builder.HasIndex(p => p.CategoryId)
                .HasDatabaseName("IX_Products_CategoryId");

            builder.HasIndex(p => p.ApprovalStatus)
                .HasDatabaseName("IX_Products_ApprovalStatus");

           
            builder.HasMany(p => p.CartItems)
                .WithOne(ci => ci.Product)
                .HasForeignKey(ci => ci.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(p => p.OrderItems)
                .WithOne(oi => oi.Product)
                .HasForeignKey(oi => oi.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(p => p.Reviews)
                .WithOne(r => r.Product)
                .HasForeignKey(r => r.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(p => p.Discounts)
                .WithOne(d => d.Product)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

                builder.HasMany(p => p.Variants)
    .WithOne(v => v.Product)
    .HasForeignKey(v => v.ProductId)
    .OnDelete(DeleteBehavior.Cascade);

builder.HasMany(p => p.CustomizationZones)
    .WithOne(z => z.Product)
    .HasForeignKey(z => z.ProductId)
    .OnDelete(DeleteBehavior.Cascade);

        }
    }
}
