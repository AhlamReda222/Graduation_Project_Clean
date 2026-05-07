using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Graduation_Project.DAL.Models.Entities;
using Graduation_Project.DAL.Models.Enums;

namespace Graduation_Project.DAL.Configurations
{
        public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
        {
            public void Configure(EntityTypeBuilder<ApplicationUser> builder)
            {
                // Custom Properties Configuration
                builder.Property(u => u.FullName)
                    .IsRequired()
                    .HasMaxLength(100);

                builder.Property(u => u.UserType)
                    .IsRequired()
                    .HasConversion<int>()
                    .HasDefaultValue(UserType.Customer);

                builder.Property(u => u.CreatedAt)
                    .IsRequired()
                    .HasDefaultValueSql("NOW()");

                builder.Property(u => u.UpdatedAt)
                    .IsRequired()
                    .HasDefaultValueSql("NOW()");

                builder.Property(u => u.IsBlocked)
                    .IsRequired()
                    .HasDefaultValue(false);

                // Indexes
                builder.HasIndex(u => u.Email)
                    .IsUnique()
                    .HasDatabaseName("IX_Users_Email");

                builder.HasIndex(u => u.UserName)
                    .IsUnique()
                    .HasDatabaseName("IX_Users_UserName");

                builder.HasIndex(u => u.UserType)
                    .HasDatabaseName("IX_Users_UserType");

                // Relationships
                builder.HasOne(u => u.Profile)
                    .WithOne(p => p.User)
                    .HasForeignKey<Profile>(p => p.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                builder.HasMany(u => u.SubmittedRequests)
                    .WithOne(r => r.User)
                    .HasForeignKey(r => r.UserId)
                    .OnDelete(DeleteBehavior.Restrict);

                builder.HasMany(u => u.ReviewedRequests)
                    .WithOne(r => r.Reviewer)
                    .HasForeignKey(r => r.ReviewedBy)
                    .OnDelete(DeleteBehavior.Restrict);

                builder.HasMany(u => u.Brands)
                    .WithOne(b => b.User)
                    .HasForeignKey(b => b.UserId)
                    .OnDelete(DeleteBehavior.Restrict);

                builder.HasMany(u => u.CartItems)
                    .WithOne(ci => ci.User)
                    .HasForeignKey(ci => ci.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                builder.HasMany(u => u.Orders)
                    .WithOne(o => o.User)
                    .HasForeignKey(o => o.UserId)
                    .OnDelete(DeleteBehavior.Restrict);

                builder.HasMany(u => u.WrittenReviews)
                    .WithOne(r => r.User)
                    .HasForeignKey(r => r.UserId)
                    .OnDelete(DeleteBehavior.Restrict);

                builder.HasMany(u => u.DeletedReviews)
                    .WithOne(r => r.DeletedByUser)
                    .HasForeignKey(r => r.DeletedBy)
                    .OnDelete(DeleteBehavior.Restrict);

                builder.HasMany(u => u.CustomerConversations)
                    .WithOne(c => c.Customer)
                    .HasForeignKey(c => c.CustomerId)
                    .OnDelete(DeleteBehavior.Restrict);

                builder.HasMany(u => u.BrandOwnerConversations)
                    .WithOne(c => c.BrandOwner)
                    .HasForeignKey(c => c.BrandOwnerId)
                    .OnDelete(DeleteBehavior.Restrict);

                builder.HasMany(u => u.Messages)
                    .WithOne(m => m.Sender)
                    .HasForeignKey(m => m.SenderId)
                    .OnDelete(DeleteBehavior.Restrict);

                builder.HasMany(u => u.ApprovedProducts)
                    .WithOne(p => p.ApprovedByUser)
                    .HasForeignKey(p => p.ApprovedBy)
                    .OnDelete(DeleteBehavior.Restrict);
            }
        }
    }
