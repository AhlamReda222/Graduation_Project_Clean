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
    public class MessageConfiguration : IEntityTypeConfiguration<Message>
    {
        public void Configure(EntityTypeBuilder<Message> builder)
        {
            builder.ToTable("Messages");

            builder.HasKey(m => m.MessageId);

            builder.Property(m => m.MessageId)
                .ValueGeneratedOnAdd();

            builder.Property(m => m.ConversationId)
                .IsRequired();

            builder.Property(m => m.SenderId)
                .IsRequired();

            builder.Property(m => m.MessageText)
                .IsRequired()
                .HasColumnType("text");

            builder.Property(m => m.SentAt)
                .IsRequired()
                .HasDefaultValueSql("(NOW())");

            builder.Property(m => m.IsRead)
                .IsRequired()
                .HasDefaultValue(false);

            // Indexes
            builder.HasIndex(m => m.ConversationId)
                .HasDatabaseName("IX_Messages_ConversationId");

            builder.HasIndex(m => m.SenderId)
                .HasDatabaseName("IX_Messages_SenderId");

            builder.HasIndex(m => m.SentAt)
                .HasDatabaseName("IX_Messages_SentAt");

            builder.HasIndex(m => m.IsRead)
                .HasDatabaseName("IX_Messages_IsRead");
        }
    }
}