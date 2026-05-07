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
    public class ConversationConfiguration : IEntityTypeConfiguration<Conversation>
    {
        public void Configure(EntityTypeBuilder<Conversation> builder)
        {
            builder.ToTable("Conversations");

            builder.HasKey(c => c.ConversationId);

            builder.Property(c => c.ConversationId)
                .ValueGeneratedOnAdd();

            builder.Property(c => c.CustomerId)
                .IsRequired();

            builder.Property(c => c.BrandOwnerId)
                .IsRequired();

            builder.Property(c => c.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("(NOW())");

            // Composite Unique Index
            builder.HasIndex(c => new { c.CustomerId, c.BrandOwnerId })
                .IsUnique()
                .HasDatabaseName("IX_Conversations_Customer_BrandOwner");

            // Indexes
            builder.HasIndex(c => c.CustomerId)
                .HasDatabaseName("IX_Conversations_CustomerId");

            builder.HasIndex(c => c.BrandOwnerId)
                .HasDatabaseName("IX_Conversations_BrandOwnerId");

            builder.HasIndex(c => c.LastMessageAt)
                .HasDatabaseName("IX_Conversations_LastMessageAt");

            // Relationships
            builder.HasMany(c => c.Messages)
                .WithOne(m => m.Conversation)
                .HasForeignKey(m => m.ConversationId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}