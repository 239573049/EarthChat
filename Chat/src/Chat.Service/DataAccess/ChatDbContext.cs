using System.Text.Json;
using Chat.Service.Domain.Chats.Aggregates;
using Chat.Service.Domain.Users.Aggregates;

namespace Chat.Service.DataAccess;

public class ChatDbContext : MasaDbContext
{
    public DbSet<User> Users { get; set; } = null!;

    public DbSet<ChatMessage> ChatMessages { get; set; } = null!;

    public ChatDbContext(MasaDbContextOptions<ChatDbContext> options) : base(options)
    {
    }

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
            
            options.Property(x => x.Cotnent).HasMaxLength(2000);
        });


        #region Init Data

        var user = new User(Guid.NewGuid())
        {
            Account = "admin",
            Password = "123456",
            Avatar = "https://avatars.githubusercontent.com/u/17716615?v=4",
            Name = "管理员",

        };

        builder.Entity<User>().HasData(user);

        #endregion
    }
}