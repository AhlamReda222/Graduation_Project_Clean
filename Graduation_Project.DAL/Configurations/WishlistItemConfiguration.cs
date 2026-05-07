using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Graduation_Project.DAL.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Graduation_Project.DAL.Configurations
{
    public class WishlistItemConfiguration : IEntityTypeConfiguration<WishlistItem>
    {
        public void Configure(EntityTypeBuilder<WishlistItem> builder)
        {
            builder.ToTable("WishlistItems");

            builder.HasKey(w => w.WishlistItemId);

            builder.Property(w => w.WishlistItemId)
                   .ValueGeneratedOnAdd();

            builder.Property(w => w.UserId)
                   .IsRequired();

            builder.Property(w => w.ProductId)
                   .IsRequired();

            builder.Property(w => w.AddedAt)
                   .IsRequired()
                   .HasDefaultValueSql("(NOW())");

            // منع إضافة نفس المنتج أكتر من مرة للـ Wishlist
            builder.HasIndex(w => new { w.UserId, w.ProductId })
                   .IsUnique()
                   .HasDatabaseName("IX_WishlistItems_User_Product");

            builder.HasOne(w => w.User)
                   .WithMany()
                   .HasForeignKey(w => w.UserId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(w => w.Product)
                   .WithMany()
                   .HasForeignKey(w => w.ProductId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}