using CommunicationsApp.Domain.Common.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CommunicationsApp.Infrastructure.Configurations;

internal sealed class MediaTypeConfiguration : IEntityTypeConfiguration<MediaType>
{
    public void Configure(EntityTypeBuilder<MediaType> builder)
    {
        builder.ToTable("MediaTypes");

        builder.HasKey(e => e.Value);

        builder.Property(e => e.Name)
               .IsRequired()
               .HasMaxLength(15);

        // Seeding
        var data = MediaType.List
                            .Select(e => new { e.Value, e.Name })
                            .ToList();

        builder.HasData(data);
    }
}
