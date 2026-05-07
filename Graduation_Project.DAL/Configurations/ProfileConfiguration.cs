using Graduation_Project.DAL.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Graduation_Project.DAL.Configurations
{
    public class ProfileConfiguration : IEntityTypeConfiguration<Profile>
    {
        public void Configure(EntityTypeBuilder<Profile> builder)
        {
            builder.ToTable("Profiles");

            builder.HasKey(p => p.ProfileId);

            builder.Property(p => p.ProfileId)
                .ValueGeneratedOnAdd();

            builder.Property(p => p.UserId)
                .IsRequired();

            builder.Property(p => p.ProfileImage)
                .HasMaxLength(500);

            builder.Property(p => p.Address)
                .HasColumnType("text");

            builder.Property(p => p.Bio)
                .HasColumnType("text");

            builder.Property(p => p.UpdatedAt)
                .IsRequired()
                .HasDefaultValueSql("(NOW())");

            builder.HasIndex(p => p.UserId)
                .IsUnique()
                .HasDatabaseName("IX_Profiles_UserId");
        }
    }
}