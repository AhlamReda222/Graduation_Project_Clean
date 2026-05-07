using Graduation_Project.DAL.Models.Enums;
using Graduation_Project.DAL.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Graduation_Project.DAL.Configurations
{
    public class BrandOwnerRequestConfiguration : IEntityTypeConfiguration<BrandOwnerRequest>
    {
        public void Configure(EntityTypeBuilder<BrandOwnerRequest> builder)
        {
            builder.ToTable("BrandOwnerRequests");

            builder.HasKey(r => r.RequestId);

            builder.Property(r => r.RequestId)
                .ValueGeneratedOnAdd();

            builder.Property(r => r.UserId)
                .IsRequired();

            builder.Property(r => r.BusinessName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(r => r.BusinessLicense)
                .IsRequired()
                .HasMaxLength(500);


            builder.Property(r => r.RequestStatus)
                .IsRequired()
                .HasConversion<int>()
                .HasDefaultValue(RequestStatus.Pending);

            builder.Property(r => r.RequestDate)
                .IsRequired()
                .HasDefaultValueSql("NOW()");

            builder.HasIndex(r => r.UserId)
                .HasDatabaseName("IX_BrandOwnerRequests_UserId");

            builder.HasIndex(r => r.RequestStatus)
                .HasDatabaseName("IX_BrandOwnerRequests_Status");
        }
    }
}