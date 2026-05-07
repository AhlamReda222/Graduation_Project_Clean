using Graduation_Project.DAL.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Graduation_Project.DAL.Configurations
{
    public class CartItemConfiguration : IEntityTypeConfiguration<CartItem>
    {
        public void Configure(EntityTypeBuilder<CartItem> builder)
        {
            builder.ToTable("CartItems");

            builder.HasKey(ci => ci.CartItemId);

            builder.Property(ci => ci.CartItemId)
                .ValueGeneratedOnAdd();

            builder.Property(ci => ci.UserId)
                .IsRequired();

            builder.Property(ci => ci.ProductId)
                .IsRequired();

            builder.Property(ci => ci.Quantity)
                .IsRequired()
                .HasDefaultValue(1);

            builder.Property(ci => ci.AddedAt)
                .IsRequired()
                .HasDefaultValueSql("NOW()");
                

            builder.HasIndex(ci => new { ci.UserId, ci.ProductId })
                .IsUnique()
                .HasDatabaseName("IX_CartItems_UserId_ProductId");
        }
    }
}