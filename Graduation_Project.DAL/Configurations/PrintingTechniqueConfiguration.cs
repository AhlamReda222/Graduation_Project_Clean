using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Graduation_Project.DAL.Models.Entities;

namespace Graduation_Project.DAL.Configurations
{
    public class PrintingTechniqueConfiguration : IEntityTypeConfiguration<PrintingTechnique>
    {
        public void Configure(EntityTypeBuilder<PrintingTechnique> builder)
        {
            builder.ToTable("PrintingTechniques");

            builder.HasKey(t => t.TechniqueId);

            builder.Property(t => t.TechniqueId)
                .ValueGeneratedOnAdd();

            builder.Property(t => t.TechniqueType)
                .IsRequired()
                .HasConversion<int>();

            builder.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(t => t.Dimensions)
                .HasMaxLength(50);

            builder.Property(t => t.Price)
                .IsRequired()
                .HasColumnType("decimal(10,2)");

            builder.Property(t => t.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            builder.HasIndex(t => t.TechniqueType)
                .IsUnique()
                .HasDatabaseName("IX_PrintingTechniques_TechniqueType");

            builder.HasMany(t => t.OrderItemCustomizations)
                .WithOne(c => c.Technique)
                .HasForeignKey(c => c.TechniqueId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}