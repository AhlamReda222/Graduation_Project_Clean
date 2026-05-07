using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Graduation_Project.DAL.Models.Entities;
using Graduation_Project.DAL.Models.Enums;

namespace Graduation_Project.DAL.Configurations
{
        public class BrandConfiguration : IEntityTypeConfiguration<Brand>
        {
            public void Configure(EntityTypeBuilder<Brand> builder)
            {
                builder.ToTable("Brands");

                builder.HasKey(b => b.BrandId);

                builder.Property(b => b.BrandId)
                    .ValueGeneratedOnAdd();

                builder.Property(b => b.UserId)
                    .IsRequired();

                builder.Property(b => b.BrandName)
                    .IsRequired()
                    .HasMaxLength(100);

                builder.Property(b => b.Description)
                    .HasColumnType("text");

                builder.Property(b => b.LogoUrl)
                    .HasMaxLength(500);

                builder.Property(b => b.CreatedAt)
                    .IsRequired()
                    .HasDefaultValueSql("NOW()");

                builder.Property(b => b.IsActive)
                    .IsRequired()
                    .HasDefaultValue(true);

                builder.HasIndex(b => b.BrandName)
                    .IsUnique()
                    .HasDatabaseName("IX_Brands_BrandName");

                builder.HasIndex(b => b.UserId)
                    .HasDatabaseName("IX_Brands_UserId");

                builder.HasMany(b => b.Products)
                    .WithOne(p => p.Brand)
                    .HasForeignKey(p => p.BrandId)
                    .OnDelete(DeleteBehavior.Restrict);
            }
        }
    }