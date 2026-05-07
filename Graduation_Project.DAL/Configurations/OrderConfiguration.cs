using Graduation_Project.DAL.Models.Entities;
using Graduation_Project.DAL.Models.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Graduation_Project.DAL.Configurations
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.ToTable("Orders");

            builder.HasKey(o => o.OrderId);

            builder.Property(o => o.OrderId)
                .ValueGeneratedOnAdd();

            builder.Property(o => o.UserId)
                .IsRequired();

            builder.Property(o => o.TotalAmount)
                .IsRequired()
                .HasColumnType("decimal(10,2)");

            builder.Property(o => o.OrderStatus)
                .IsRequired()
                .HasConversion<int>()
                .HasDefaultValue(OrderStatus.Pending);

            builder.Property(o => o.ShippingAddress)
                .IsRequired()
                .HasColumnType("text");

            builder.Property(o => o.PaymentMethod)
                .IsRequired()
                .HasConversion<int>();

            builder.Property(o => o.PaymentStatus)
                .IsRequired()
                .HasConversion<int>()
                .HasDefaultValue(PaymentStatus.Pending);

            builder.Property(o => o.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("NOW()");

            builder.Property(o => o.UpdatedAt)
                .IsRequired()
                .HasDefaultValueSql("NOW()");

            builder.Property(o => o.TrackingNumber)
                .HasMaxLength(100);

            // Indexes
            builder.HasIndex(o => o.UserId)
                .HasDatabaseName("IX_Orders_UserId");

            builder.HasIndex(o => o.OrderStatus)
                .HasDatabaseName("IX_Orders_OrderStatus");

            builder.HasIndex(o => o.CreatedAt)
                .HasDatabaseName("IX_Orders_CreatedAt");

            // Relationships
            builder.HasMany(o => o.OrderItems)
                .WithOne(oi => oi.Order)
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(o => o.Reviews)
                .WithOne(r => r.Order)
                .HasForeignKey(r => r.OrderId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}