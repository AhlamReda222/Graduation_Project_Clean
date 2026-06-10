// FILE: DAL/Configurations/UserBehaviorEventConfiguration.cs
using Graduation_Project.DAL.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Graduation_Project.DAL.Configurations
{
    public class UserBehaviorEventConfiguration : IEntityTypeConfiguration<UserBehaviorEvent>
    {
        public void Configure(EntityTypeBuilder<UserBehaviorEvent> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .HasDefaultValueSql("gen_random_uuid()"); // PostgreSQL

            builder.Property(x => x.SessionId)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(x => x.EventType)
                .IsRequired()
                .HasConversion<int>();

            builder.Property(x => x.SearchQuery)
                .HasMaxLength(500);

            builder.Property(x => x.SourcePage)
                .HasMaxLength(100);

            builder.Property(x => x.CreatedAt)
                .IsRequired();

            // ✅ FK → ApplicationUser (nullable)
            // لو User اتحذف → SET NULL مش CASCADE
            builder.HasOne(x => x.User)
                .WithMany(u => u.BehaviorEvents)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.SetNull)
                .IsRequired(false);

            // Indexes للـ queries السريعة
            builder.HasIndex(x => x.UserId);
            builder.HasIndex(x => x.SessionId);
            builder.HasIndex(x => x.EventType);
            builder.HasIndex(x => x.ProductId);
            builder.HasIndex(x => x.CategoryId);
            builder.HasIndex(x => x.BrandId);
            builder.HasIndex(x => x.CreatedAt);

            // ✅ Composite index للـ AI queries (userId + eventType + date)
            builder.HasIndex(x => new { x.UserId, x.EventType, x.CreatedAt });
            builder.HasIndex(x => new { x.SessionId, x.EventType });
        }
    }
}