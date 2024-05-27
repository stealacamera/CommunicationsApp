using CommunicationsApp.Application.Common.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CommunicationsApp.Infrastructure.Configurations;

internal sealed class ChannelRoleConfiguration : IEntityTypeConfiguration<ChannelRole>
{
    public void Configure(EntityTypeBuilder<ChannelRole> builder)
    {
        builder.ToTable("ChannelRoles");

        builder.HasKey(e => e.Value);
        
        builder.Property(e => e.Name)
               .IsRequired()
               .HasMaxLength(30);

        // Seeding
        var data = ChannelRole.List
                              .Select(e => new { e.Value, e.Name })
                              .ToList();

        builder.HasData(data);
    }
}
