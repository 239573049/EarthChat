using EarthChat.Auth.Domains;
using Microsoft.EntityFrameworkCore;

namespace EarthChat.Auth.EntityFrameworkCore;

public static class ConfigureEntityExtensions
{
    public static void ConfigureEntities(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<EarthUser>(builder =>
        {
            builder.ToTable("Users");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.UserName)
                .HasMaxLength(256)
                .IsRequired();

            builder.Property(x => x.NormalizedUserName)
                .HasMaxLength(256)
                .IsRequired();

            builder.Property(x => x.Email)
                .HasMaxLength(256);

            builder.Property(x => x.NormalizedEmail)
                .HasMaxLength(256);

            builder.Property(x => x.EmailConfirmed)
                .IsRequired();

            builder.Property(x => x.PasswordHash)
                .HasMaxLength(256);

            builder.HasIndex(x => new
            {
                x.UserName,
                x.Email,
                x.PhoneNumber
            }).IsUnique();
        });
    }
}