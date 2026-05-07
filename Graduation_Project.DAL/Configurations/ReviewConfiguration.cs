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
    public class ReviewConfiguration : IEntityTypeConfiguration<Review>
    {
        public void Configure(EntityTypeBuilder<Review> builder)
        {
            builder.ToTable("Reviews");

            builder.HasKey(r => r.ReviewId);

            builder.Property(r => r.ReviewId)
                .ValueGeneratedOnAdd();

            builder.Property(r => r.ProductId)
                .IsRequired();

            builder.Property(r => r.UserId)
                .IsRequired();

            builder.Property(r => r.OrderId)
                .IsRequired();

            builder.Property(r => r.Rating)
                .IsRequired();

            builder.Property(r => r.ReviewText)
                .HasColumnType("text");

            builder.Property(r => r.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("NOW()");

            builder.Property(r => r.IsDeleted)
                .IsRequired()
                .HasDefaultValue(false);

            // Indexes
            builder.HasIndex(r => r.ProductId)
                .HasDatabaseName("IX_Reviews_ProductId");

            builder.HasIndex(r => r.UserId)
                .HasDatabaseName("IX_Reviews_UserId");

            builder.HasIndex(r => r.Rating)
                .HasDatabaseName("IX_Reviews_Rating");

            builder.HasIndex(r => r.IsDeleted)
                .HasDatabaseName("IX_Reviews_IsDeleted");

            builder.HasCheckConstraint(
                "CK_Reviews_Rating",
                "\"Rating\" BETWEEN 1 AND 5"
                    );
        }
    }
}