using CommunicationsApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CommunicationsApp.Infrastructure.Configurations;

internal class MessageConfiguration : IEntityTypeConfiguration<Message>
{
    public void Configure(EntityTypeBuilder<Message> builder)
    {
        builder.ToTable("Messages");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.CreatedAt)
               .IsRequired();

        builder.Property(e => e.Text)
               .HasMaxLength(1000)
               .IsRequired();

        builder.HasOne<User>()
               .WithMany()
               .HasForeignKey(e => e.OwnerId)
               .OnDelete(DeleteBehavior.Restrict)
               .IsRequired();

        builder.HasOne<Channel>()
               .WithMany()
               .HasForeignKey(e => e.ChannelId)
               .OnDelete(DeleteBehavior.Restrict)
               .IsRequired();
    }
}
