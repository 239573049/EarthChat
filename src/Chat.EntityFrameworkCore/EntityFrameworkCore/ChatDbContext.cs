using Chat.Chat;
using Chat.Users;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.AuditLogging.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.Modeling;
using Volo.Abp.Identity;
using Volo.Abp.Identity.EntityFrameworkCore;
using Volo.Abp.OpenIddict.EntityFrameworkCore;
using Volo.Abp.PermissionManagement.EntityFrameworkCore;
using Volo.Abp.SettingManagement.EntityFrameworkCore;

namespace Chat.EntityFrameworkCore;

[ReplaceDbContext(typeof(IIdentityDbContext))]
[ConnectionStringName("Default")]
public class ChatDbContext :
    AbpDbContext<ChatDbContext>,
    IIdentityDbContext
{
    /* Add DbSet properties for your Aggregate Roots / Entities here. */

    #region Entities from the modules

    //Identity
    public DbSet<IdentityUser> Users { get; set; }
    public DbSet<IdentityRole> Roles { get; set; }
    public DbSet<IdentityClaimType> ClaimTypes { get; set; }
    public DbSet<OrganizationUnit> OrganizationUnits { get; set; }
    public DbSet<IdentitySecurityLog> SecurityLogs { get; set; }
    public DbSet<IdentityLinkUser> LinkUsers { get; set; }
    public DbSet<IdentityUserDelegation> UserDelegations { get; set; }

    #endregion

    #region Chat

    public DbSet<ChatUser> ChatUsers { get; set; }

    public DbSet<ChatGroup> ChatGroups { get; set; }

    public DbSet<ChatGroupInUser> ChatGroupInUsers { get; set; }

    #endregion

    public ChatDbContext(DbContextOptions<ChatDbContext> options)
        : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        /* Include modules to your migration db context */

        builder.ConfigurePermissionManagement();
        builder.ConfigureSettingManagement();
        builder.ConfigureAuditLogging();
        builder.ConfigureIdentity();
        builder.ConfigureOpenIddict();

        builder.Entity<ChatUser>(options =>
        {
            options.HasIndex(x => x.GitHubId);

            options.HasIndex(x => x.GiteeId);
        });

        builder.Entity<ChatMessage>(options =>
        {
            options.ConfigureAuditedAggregateRoot();
            options.HasIndex(x => x.ChatGroupId);
            options.HasIndex(x => x.UserId);

        });

        builder.Entity<ChatGroup>(options =>
        {
            options.ConfigureAuditedAggregateRoot();
        });

        builder.Entity<ChatGroupInUser>(options =>
        {
            options.ConfigureAuditedAggregateRoot();
        });
    }
}
