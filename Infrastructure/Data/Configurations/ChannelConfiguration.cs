﻿using CommunicationsApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CommunicationsApp.Infrastructure.Data.Configurations;

internal class ChannelConfiguration : IEntityTypeConfiguration<Channel>
{
    public void Configure(EntityTypeBuilder<Channel> builder)
    {
        builder.ToTable("Channels");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Name)
               .IsRequired()
               .HasMaxLength(55);

        builder.Property(e => e.Code)
               .IsRequired()
               .HasMaxLength(10);

        builder.HasIndex(e => e.Code)
               .IsUnique();

        builder.Property(e => e.CreatedAt)
               .IsRequired();

        builder.HasOne<User>()
               .WithMany()
               .HasForeignKey(e => e.OwnerId)
               .OnDelete(DeleteBehavior.Restrict)
               .IsRequired();

        builder.HasMany(e => e.Members)
               .WithMany();
    }
}