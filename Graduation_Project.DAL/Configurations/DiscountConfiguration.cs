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
    public class DiscountConfiguration : IEntityTypeConfiguration<Discount>
    {
        public void Configure(EntityTypeBuilder<Discount> builder)
        {
            builder.ToTable("Discounts");

            builder.HasKey(d => d.DiscountId);

            builder.Property(d => d.DiscountId)
                .ValueGeneratedOnAdd();

            builder.Property(d => d.ProductId)
                .IsRequired();

            builder.Property(d => d.DiscountType)
                .IsRequired()
                .HasConversion<int>();

            builder.Property(d => d.DiscountValue)
                .IsRequired()
                .HasColumnType("decimal(10,2)");

            builder.Property(d => d.StartDate)
                .IsRequired();

            builder.Property(d => d.EndDate)
                .IsRequired();

            builder.Property(d => d.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            // Indexes
            builder.HasIndex(d => d.ProductId)
                .HasDatabaseName("IX_Discounts_ProductId");

            builder.HasIndex(d => new { d.StartDate, d.EndDate })
                .HasDatabaseName("IX_Discounts_Dates");

            builder.HasIndex(d => d.IsActive)
                .HasDatabaseName("IX_Discounts_IsActive");
        }
    }
}