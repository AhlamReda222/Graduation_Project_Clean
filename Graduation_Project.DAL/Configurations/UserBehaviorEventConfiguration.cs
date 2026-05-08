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

            builder.HasIndex(x => x.UserId);
            builder.HasIndex(x => x.SessionId);
            builder.HasIndex(x => x.EventType);
            builder.HasIndex(x => x.ProductId);
            builder.HasIndex(x => x.CategoryId);
            builder.HasIndex(x => x.BrandId);
            builder.HasIndex(x => x.CreatedAt);
        }
    }
}