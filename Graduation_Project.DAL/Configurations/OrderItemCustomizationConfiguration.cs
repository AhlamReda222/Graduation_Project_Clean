using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Graduation_Project.DAL.Models.Entities;

namespace Graduation_Project.DAL.Configurations
{
    public class OrderItemCustomizationConfiguration : IEntityTypeConfiguration<OrderItemCustomization>
    {
        public void Configure(EntityTypeBuilder<OrderItemCustomization> builder)
        {
            builder.ToTable("OrderItemCustomizations");

            builder.HasKey(c => c.CustomizationId);

            builder.Property(c => c.CustomizationId)
                .ValueGeneratedOnAdd();

            builder.Property(c => c.OrderItemId)
                .IsRequired();

            builder.Property(c => c.Zone)
                .IsRequired()
                .HasConversion<int>();

            builder.Property(c => c.TechniqueId)
                .IsRequired();

            builder.Property(c => c.DesignImageUrl)
                .HasColumnType("text");

            builder.Property(c => c.DesignText)
                .HasMaxLength(500);

            builder.Property(c => c.CustomizationPrice)
                .IsRequired()
                .HasColumnType("decimal(10,2)");

            builder.HasIndex(c => c.OrderItemId)
                .HasDatabaseName("IX_OrderItemCustomizations_OrderItemId");

            builder.HasOne(c => c.OrderItem)
                .WithMany(oi => oi.Customizations)
                .HasForeignKey(c => c.OrderItemId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(c => c.Technique)
                .WithMany(t => t.OrderItemCustomizations)
                .HasForeignKey(c => c.TechniqueId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(c => c.DesignImageUrl)
    .IsRequired(false);  // ✅

builder.Property(c => c.DesignText)
    .IsRequired(false);  // ✅
        }
    }
}