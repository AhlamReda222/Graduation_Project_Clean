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
    public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
    {
        public void Configure(EntityTypeBuilder<OrderItem> builder)
        {
            builder.ToTable("OrderItems");

            builder.HasKey(oi => oi.OrderItemId);

            builder.Property(oi => oi.OrderItemId)
                .ValueGeneratedOnAdd();

            builder.Property(oi => oi.OrderId)
                .IsRequired();

            builder.Property(oi => oi.ProductId)
                .IsRequired();

            builder.Property(oi => oi.Quantity)
                .IsRequired();

            builder.Property(oi => oi.UnitPrice)
                .IsRequired()
                .HasColumnType("decimal(10,2)");

            builder.Property(oi => oi.Subtotal)
                .IsRequired()
                .HasColumnType("decimal(10,2)");

            // Indexes
            builder.HasIndex(oi => oi.OrderId)
                .HasDatabaseName("IX_OrderItems_OrderId");

            builder.HasIndex(oi => oi.ProductId)
                .HasDatabaseName("IX_OrderItems_ProductId");
        }
    }
}
