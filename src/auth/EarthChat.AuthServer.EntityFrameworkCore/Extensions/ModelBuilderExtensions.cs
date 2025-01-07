using EarthChat.AuthServer.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace EarthChat.AuthServer.EntityFrameworkCore.Extensions;

public static class ModelBuilderExtensions
{
    public static ModelBuilder ConfigureAuth(this ModelBuilder model)
    {
        model.Entity<User>(builder =>
        {
            builder.ToTable("users");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Username)
                .IsRequired()
                .HasMaxLength(32)
                .HasComment("用户名");

            builder.Property(x => x.PasswordHash)
                .IsRequired()
                .HasMaxLength(64)
                .HasComment("密码哈希值");

            builder.Property(x => x.PasswordSalt)
                .IsRequired();

            builder.Property(x => x.Email)
                .IsRequired()
                .HasMaxLength(64)
                .HasComment("邮箱");

            builder.Property(x => x.Avatar)
                .HasMaxLength(256)
                .HasComment("头像");

            builder.HasIndex(x => x.Username)
                .IsUnique();

            builder.HasIndex(x => x.Email)
                .IsUnique();

            builder.Property(x => x.Signature)
                .HasMaxLength(500)
                .HasComment("个性签名");
        });

        model.Entity<UserOAuth>(builder =>
        {
            builder.ToTable("user-oauth");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Provider)
                .IsRequired()
                .HasComment("OAuth 提供者");

            builder.Property(x => x.ProviderUserId)
                .HasComment("OAuth 提供者用户 ID");

            builder.HasIndex(x => x.UserId);

            // 聚合索引
            builder.HasIndex(x => new { x.Provider, x.ProviderUserId })
                .IsUnique();

            builder.HasIndex(x => new { x.Provider, x.ProviderUserId, x.UserId });
        });

        return model;
    }
}