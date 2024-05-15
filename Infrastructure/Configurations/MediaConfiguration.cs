using CommunicationsApp.Domain.Common.Enums;
using CommunicationsApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CommunicationsApp.Infrastructure.Configurations;

internal sealed class MediaConfiguration : IEntityTypeConfiguration<Media>
{
    public void Configure(EntityTypeBuilder<Media> builder)
    {
        builder.ToTable("Media");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Filename)
               .IsRequired();

        builder.HasOne<Message>()
               .WithMany()
               .HasForeignKey(e => e.MessageId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<MediaType>()
               .WithMany()
               .HasForeignKey(e => e.MediaTypeId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Restrict);
    }
}