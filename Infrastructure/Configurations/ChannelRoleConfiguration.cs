using CommunicationsApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CommunicationsApp.Infrastructure.Configurations;

internal sealed class ChannelRoleConfiguration : IEntityTypeConfiguration<ChannelRole>
{
    public void Configure(EntityTypeBuilder<ChannelRole> builder)
    {
        builder.ToTable("ChannelRoles");

        builder.HasKey(e => e.Id);
        
        builder.Property(e => e.Name)
               .IsRequired()
               .HasMaxLength(30);

        // Seeding
        var data = Application.Common.Enums.ChannelRole.List
                                                       .Select(e => new ChannelRole
                                                       {
                                                           Id = e.Value,
                                                           Name = e.Name
                                                       })
                                                       .ToList();

        builder.HasData(data);
    }
}
