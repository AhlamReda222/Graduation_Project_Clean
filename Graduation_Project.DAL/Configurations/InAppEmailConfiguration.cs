using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Graduation_Project.DAL.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Graduation_Project.DAL.Configurations
{
    public class InAppEmailConfiguration : IEntityTypeConfiguration<InAppEmail>
    {
        public void Configure(EntityTypeBuilder<InAppEmail> builder)
        {
            builder.ToTable("InAppEmails");

            builder.HasKey(e => e.EmailId);

            builder.Property(e => e.Subject)
                   .IsRequired()
                   .HasMaxLength(300);

            builder.Property(e => e.Body)
                   .IsRequired();

            builder.Property(e => e.IsRead)
                   .HasDefaultValue(false);

            builder.Property(e => e.CreatedAt)
                   .IsRequired()
                   .HasDefaultValueSql("(NOW())");

            builder.HasOne(e => e.User)
                   .WithMany()
                   .HasForeignKey(e => e.UserId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(e => e.UserId)
                   .HasDatabaseName("IX_InAppEmails_UserId");
        }
    }
}