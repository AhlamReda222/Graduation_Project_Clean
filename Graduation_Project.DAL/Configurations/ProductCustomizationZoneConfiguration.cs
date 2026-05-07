using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Graduation_Project.DAL.Models.Entities;

namespace Graduation_Project.DAL.Configurations
{
    public class ProductCustomizationZoneConfiguration : IEntityTypeConfiguration<ProductCustomizationZone>
    {
        public void Configure(EntityTypeBuilder<ProductCustomizationZone> builder)
        {
            builder.ToTable("ProductCustomizationZones");

            builder.HasKey(z => z.ZoneId);

            builder.Property(z => z.ZoneId)
                .ValueGeneratedOnAdd();

            builder.Property(z => z.ProductId)
                .IsRequired();

            builder.Property(z => z.Zone)
                .IsRequired()
                .HasConversion<int>();

            builder.Property(z => z.IsAvailable)
                .IsRequired()
                .HasDefaultValue(true);

            builder.HasIndex(z => z.ProductId)
                .HasDatabaseName("IX_ProductCustomizationZones_ProductId");

            builder.HasIndex(z => new { z.ProductId, z.Zone })
                .IsUnique()
                .HasDatabaseName("IX_ProductCustomizationZones_ProductId_Zone");

            builder.HasOne(z => z.Product)
                .WithMany(p => p.CustomizationZones)
                .HasForeignKey(z => z.ProductId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}