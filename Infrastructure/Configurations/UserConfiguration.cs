using CommunicationsApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CommunicationsApp.Infrastructure.Configurations;

internal class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.Property(e => e.UserName)
               .IsRequired()
               .HasMaxLength(45);

        builder.Property(e => e.CreatedAt)
               .IsRequired();
    }
}
