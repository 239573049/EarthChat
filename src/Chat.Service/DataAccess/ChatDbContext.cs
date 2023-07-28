using Chat.Service.Domain.Chats.Aggregates;
using Chat.Service.Domain.Users.Aggregates;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;

namespace Chat.Service.DataAccess;

public class ChatDbContext : MasaDbContext
{
    public ChatDbContext(MasaDbContextOptions<ChatDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; } = null!;

    public DbSet<ChatMessage> ChatMessages { get; set; } = null!;

    protected override void OnModelCreatingExecuting(ModelBuilder modelBuilder)
    {
        base.OnModelCreatingExecuting(modelBuilder);
        ConfigEntities(modelBuilder);
    }

    private static void ConfigEntities(ModelBuilder builder)
    {
        builder.Entity<User>(options =>
        {
            options.TryConfigureConcurrencyStamp();
            options.HasIndex(x => x.Id);
            options.HasIndex(x => x.Account).IsUnique();

            options.Property(x => x.Account).HasMaxLength(20).IsRequired();

            options.Property(x => x.Password).HasMaxLength(20).IsRequired();

            options.Property(x => x.Avatar).HasMaxLength(200).IsRequired();

            options.Property(x => x.Name).HasMaxLength(20).IsRequired();
        });

        builder.Entity<ChatMessage>(options =>
        {
            options.TryConfigureConcurrencyStamp();
            options.HasIndex(x => x.Id);

            options.Property(x => x.Content).HasMaxLength(2000);

            // 设置userId为逻辑外键

            options.HasOne(o => o.User)
                .WithMany()
                .HasForeignKey(o => o.UserId)
                .HasConstraintName("UserId");
        });


        #region Init Data

        var user = new User(Guid.NewGuid())
        {
            Account = "admin",
            Password = "123456",
            Avatar = "https://avatars.githubusercontent.com/u/17716615?v=4",
            Name = "管理员"
        };

        builder.Entity<User>().HasData(user);

        #endregion
    }
}