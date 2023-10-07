using Chat.Service.Domain.Chats.Aggregates;
using Chat.Service.Domain.System.Aggregates;
using Chat.Service.Domain.Users.Aggregates;
using Chat.Service.Infrastructure.Helper;
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

    public DbSet<ChatGroup> ChatGroups { get; set; } = null!;

    public DbSet<ChatGroupInUser> ChatGroupInUsers { get; set; } = null!;

    public DbSet<FileSystem> FileSystems { get; set; } = null!;

    public DbSet<FriendRequest> FriendRequests { get; set; }

    public DbSet<Friend> Friends { get; set; }

    public DbSet<Emoji> Emojis { get; set; }

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

            options.Property(x => x.Password).HasMaxLength(80).IsRequired();

            options.Property(x => x.Avatar).HasMaxLength(200).IsRequired();

            options.Property(x => x.Name).HasMaxLength(20).IsRequired();

            // efcore转换器，在保存到数据库会将当前属性的值进行转换。
            options.Property(x => x.Password)
                .HasConversion(x => EncryptHelper.Encrypt(x), x => EncryptHelper.Decrypt(x));
        });

        builder.Entity<ChatMessage>(options =>
        {
            options.TryConfigureConcurrencyStamp();
            options.HasIndex(x => x.Id);

            options.Property(x => x.Content).HasMaxLength(5000);

            options.HasIndex(x => x.ChatGroupId);
            options.HasIndex(x => x.UserId);
            options.HasIndex(x => x.RevertId);

        });

        builder.Entity<ChatGroup>(options =>
        {
            options.TryConfigureConcurrencyStamp();

            options.HasIndex(x => x.Id);

            options.Property(x => x.Name).HasMaxLength(20).IsRequired();

            options.Property(x => x.Avatar).HasMaxLength(200).IsRequired();

            options.Property(x => x.Description).HasMaxLength(2000);
        });

        builder.Entity<ChatGroupInUser>(options =>
        {
            options.HasKey(x => x.Id);
            options.HasIndex(x => x.Id);

            options.HasKey(x => new { x.UserId, x.ChatGroupId });

            options.HasOne(o => o.User)
                .WithMany()
                .HasForeignKey(o => o.UserId)
                .HasConstraintName("UserId");

            options.HasOne(o => o.ChatGroup)
                .WithMany()
                .HasForeignKey(o => o.ChatGroupId)
                .HasConstraintName("ChatGroupId");
        });

        builder.Entity<FileSystem>(options =>
        {
            options.TryConfigureConcurrencyStamp();

            options.HasKey(x => x.Id);

            options.HasIndex(x => x.Id);
        });

        builder.Entity<Friend>(options =>
        {
            options.TryConfigureConcurrencyStamp();

            options.HasKey(x => x.Id);
            options.HasIndex(x => x.Id);

            options.HasIndex(x => x.SelfId);
        });

        builder.Entity<FriendRequest>(options =>
        {
            options.TryConfigureConcurrencyStamp();

            options.HasKey(x => x.Id);
            options.HasIndex(x => x.Id);

            options.HasIndex(x => x.RequestId);
            options.HasIndex(x => x.BeAppliedForId);
        });

        builder.Entity<Emoji>(options =>
        {
            options.HasKey(x => x.Id);
            options.HasIndex(x => x.Id);

            options.HasIndex(x => x.UserId);
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

        // TODO: 定义空的Guid，用于表示世界频道
        var group = new ChatGroup(Guid.NewGuid())
        {
            Name = "世界频道",
            Default = true,
            Avatar = "https://avatars.githubusercontent.com/u/17716615?v=4",
            Description = "世界频道，所有人默认加入的频道"
        };

        builder.Entity<ChatGroup>().HasData(group);


        var groupInUser = new ChatGroupInUser()
        {
            UserId = user.Id,
            ChatGroupId = group.Id
        };

        builder.Entity<ChatGroupInUser>().HasData(groupInUser);

        #endregion
    }
}