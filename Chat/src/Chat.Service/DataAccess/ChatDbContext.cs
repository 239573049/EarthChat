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

            options.Property(x => x.Extends).HasConversion(
                v => JsonSerializer.Serialize(v, new JsonSerializerOptions()),
                v => JsonSerializer.Deserialize<Dictionary<string, string>>(v, new JsonSerializerOptions()));
        });

        builder.Entity<ChatMessage>(options =>
        {
            options.TryConfigureConcurrencyStamp();
            options.HasIndex(x => x.Id);
            
            options.Property(x => x.Cotnent).HasMaxLength(2000);
            
        });
    }
}