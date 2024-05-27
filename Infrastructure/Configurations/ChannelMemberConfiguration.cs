using CommunicationsApp.Application.Common.Enums;
using CommunicationsApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CommunicationsApp.Infrastructure.Configurations;

internal class ChannelMemberConfiguration : IEntityTypeConfiguration<ChannelMember>
{
    public void Configure(EntityTypeBuilder<ChannelMember> builder)
    {
        builder.ToTable("ChannelMembers");

        builder.HasKey(e => new { e.MemberId, e.ChannelId });

        builder.HasOne<User>()
               .WithMany()
               .HasForeignKey(e => e.MemberId)
               .OnDelete(DeleteBehavior.Restrict)
               .IsRequired();

        builder.HasOne<Channel>()
               .WithMany()
               .HasForeignKey(e => e.ChannelId)
               .OnDelete(DeleteBehavior.Restrict)
               .IsRequired();

        builder.HasOne<ChannelRole>()
               .WithMany()
               .HasForeignKey(e => e.RoleId)
               .OnDelete(DeleteBehavior.Restrict)
               .IsRequired();
    }
}
