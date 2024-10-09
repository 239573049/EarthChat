using System.Linq.Expressions;
using System.Text.Json;
using System.Text.Json.Serialization;
using EarthChat.Ddd;
using EarthChat.EntityFrameworkCore.Options;
using EarthChat.Identity.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace EarthChat.EntityFrameworkCore;

/// <summary>
/// A generic DbContext for EarthChat.
/// </summary>
/// <param name="options"></param>
/// <typeparam name="TDbContext"></typeparam>
public abstract class EarthDbContext<TDbContext>(
    DbContextOptions<TDbContext> options,
    IServiceProvider serviceProvider)
    : DbContext(options) where TDbContext : DbContext
{
    private IUserContext UserContext => serviceProvider.GetRequiredService<IUserContext>();

    private IOptions<EarthDbContextOptions> EarthDbContextOptions =>
        serviceProvider.GetRequiredService<IOptions<EarthDbContextOptions>>();

    protected virtual JsonSerializerOptions SerializerOptions => new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        AddSoftDeleteFilter(modelBuilder);
        AddTenantFilter(modelBuilder);
        AddExtendPropertyConvert(modelBuilder);

        UseLowerCaseNamingConvention(modelBuilder);

        base.OnModelCreating(modelBuilder);
    }

    /// <summary>
    /// 使用小写命名约定
    /// </summary>
    /// <param name="modelBuilder"></param>
    protected virtual void UseLowerCaseNamingConvention(ModelBuilder modelBuilder)
    {
        foreach (var entity in modelBuilder.Model.GetEntityTypes())
        {
            // 表名转换为小写
            entity.SetTableName(entity.GetTableName()?.ToLower() ?? entity.GetDefaultTableName()?.ToLower());

            foreach (var property in entity.GetProperties())
            {
                property.SetColumnName(property.Name.ToLower());
            }
        }
    }

    /// <summary>
    /// 给所有扩展属性添加转换
    /// </summary>
    /// <param name="modelBuilder"></param>
    protected virtual void AddExtendPropertyConvert(ModelBuilder modelBuilder)
    {
        var entityTypes = modelBuilder.Model.GetEntityTypes().ToList();

        foreach (var entityType in
                 entityTypes.Where(entityType => typeof(IExtend).IsAssignableFrom(entityType.ClrType)))
        {
            // 默认情况使用数据库的json字段类型，如果不支持则使用json序列化
            modelBuilder.Entity(entityType.ClrType)
                .Property<IDictionary<string, string>>(nameof(IExtend.Extend))
                .HasColumnType("jsonb")
                .HasConversion(
                    v => JsonSerializer.Serialize(v, SerializerOptions),
                    v => JsonSerializer.Deserialize<IDictionary<string, string>>(v, SerializerOptions) ??
                         new Dictionary<string, string>()
                )
                .Metadata.SetValueConverter(
                    new ValueConverter<IDictionary<string, string>, string>(
                        v => JsonSerializer.Serialize(v, SerializerOptions),
                        v => JsonSerializer.Deserialize<IDictionary<string, string>>(v, SerializerOptions) ??
                             new Dictionary<string, string>()
                    )
                );
        }
    }

    /// <summary>
    /// 添加多租户过滤器
    /// </summary>
    /// <param name="modelBuilder"></param>
    protected virtual void AddTenantFilter(ModelBuilder modelBuilder)
    {
        if (EarthDbContextOptions.Value.EnableMultiTenancy == false)
        {
            return;
        }

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(IHasTenant).IsAssignableFrom(entityType.ClrType))
            {
                var parameter = Expression.Parameter(entityType.ClrType, "tenant");
                var property = Expression.Property(parameter, nameof(IHasTenant.TenantId));
                var filter = Expression.Lambda(
                    Expression.Equal(property, Expression.Constant(UserContext.CurrentTenantId)),
                    parameter);

                modelBuilder.Entity(entityType.ClrType).HasQueryFilter(filter);
            }
        }
    }

    /// <summary>
    /// 给实体添加软删除过滤器
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    protected virtual void AddSoftDeleteFilter(ModelBuilder modelBuilder)
    {
        if (EarthDbContextOptions.Value.EnableSoftDelete == false)
        {
            return;
        }

        var entityTypes = modelBuilder.Model.GetEntityTypes().ToList();

        foreach (var entityType in entityTypes)
        {
            if (typeof(ISoftDelete).IsAssignableFrom(entityType.ClrType))
            {
                var parameter = Expression.Parameter(entityType.ClrType, "deleted");
                var property = Expression.Property(parameter, nameof(ISoftDelete.IsDeleted));
                var filter = Expression.Lambda(Expression.Equal(property, Expression.Constant(false)), parameter);

                modelBuilder.Entity(entityType.ClrType).HasQueryFilter(filter);
            }
        }
    }

    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        BeforeSaveChanges();
        return base.SaveChanges(acceptAllChangesOnSuccess);
    }

    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess,
        CancellationToken cancellationToken = new())
    {
        BeforeSaveChanges();
        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    /// <summary>
    /// 设置相关数据
    /// </summary>
    protected virtual void BeforeSaveChanges()
    {
        if (!EarthDbContextOptions.Value.EnableAudit)
        {
            return;
        }

        var entries = ChangeTracker.Entries().ToList();

        var now = DateTime.Now;

        foreach (var entry in entries)
        {
            switch (entry.State)
            {
                case EntityState.Added:
                {
                    // 判断entry.Entity是否实现了entityType泛型接口
                    if (entry.Entity is ICreatable creatable)
                    {
                        creatable.CreatedAt = now;
                    }

                    if (UserContext is { IsAuthenticated: true, CurrentUserId: not null })
                    {
                        switch (entry.Entity)
                        {
                            case ICreatable<string> creatableUser:
                                creatableUser.CreatedBy = UserContext.CurrentUserId;
                                break;
                            case ICreatable<Guid> creatableGuid:
                                creatableGuid.CreatedBy = Guid.Parse(UserContext.CurrentUserId);
                                break;
                            case ICreatable<int> creatableInt:
                                creatableInt.CreatedBy = int.Parse(UserContext.CurrentUserId);
                                break;
                            case ICreatable<long> creatableLong:
                                creatableLong.CreatedBy = long.Parse(UserContext.CurrentUserId);
                                break;
                        }
                    }

                    break;
                }
                case EntityState.Modified:
                {
                    if (entry.Entity is IUpdatable updatable)
                    {
                        updatable.UpdatedAt = now;
                    }

                    if (UserContext is { IsAuthenticated: true, CurrentUserId: not null })
                    {
                        switch (entry.Entity)
                        {
                            case IUpdatable<string> updatableUser:
                                updatableUser.UpdatedBy = UserContext.CurrentUserId;
                                break;
                            case IUpdatable<Guid> updatableGuid:
                                updatableGuid.UpdatedBy = Guid.Parse(UserContext.CurrentUserId);
                                break;
                            case IUpdatable<int> updatableInt:
                                updatableInt.UpdatedBy = int.Parse(UserContext.CurrentUserId);
                                break;
                            case IUpdatable<long> updatableLong:
                                updatableLong.UpdatedBy = long.Parse(UserContext.CurrentUserId);
                                break;
                        }
                    }

                    break;
                }
                case EntityState.Detached:
                    break;
                case EntityState.Unchanged:
                    break;
                case EntityState.Deleted:
                    if (entry.Entity is ISoftDelete softDelete)
                    {
                        softDelete.IsDeleted = true;
                    }

                    entry.State = EntityState.Modified;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}