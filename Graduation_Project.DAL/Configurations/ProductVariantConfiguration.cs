using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Graduation_Project.DAL.Models.Entities;

namespace Graduation_Project.DAL.Configurations
{
    public class ProductVariantConfiguration : IEntityTypeConfiguration<ProductVariant>
    {
        public void Configure(EntityTypeBuilder<ProductVariant> builder)
        {
            builder.ToTable("ProductVariants");

            builder.HasKey(v => v.VariantId);

            builder.Property(v => v.VariantId)
                .ValueGeneratedOnAdd();

            builder.Property(v => v.ProductId)
                .IsRequired();

            builder.Property(v => v.Size)
                .IsRequired()
                .HasMaxLength(10);

            builder.Property(v => v.Color)
                .HasMaxLength(50);

            builder.Property(v => v.Price)
                .IsRequired()
                .HasColumnType("decimal(10,2)");

            builder.Property(v => v.StockQuantity)
                .IsRequired()
                .HasDefaultValue(0);

            builder.Property(v => v.SKU)
                .HasMaxLength(100);

            builder.HasIndex(v => v.ProductId)
                .HasDatabaseName("IX_ProductVariants_ProductId");

            builder.HasIndex(v => v.SKU)
                .IsUnique()
                .HasDatabaseName("IX_ProductVariants_SKU");

            builder.HasOne(v => v.Product)
                .WithMany(p => p.Variants)
                .HasForeignKey(v => v.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(v => v.CartItems)
                .WithOne(ci => ci.ProductVariant)
                .HasForeignKey(ci => ci.VariantId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(v => v.OrderItems)
                .WithOne(oi => oi.ProductVariant)
                .HasForeignKey(oi => oi.VariantId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}