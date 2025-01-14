﻿using EarthChat.Core.Contract;
using EarthChat.Domain.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;

namespace EarthChat.EntityFrameworkCore;

public abstract class MasterDbContext<TDbContext>(
    DbContextOptions<TDbContext> options,
    IServiceProvider serviceProvider)
    : DbContext(options) where TDbContext :
    DbContext
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        // 忽略未应用的模型更改警告，这个是.net9出现的新警告，不忽略会影响迁移记录应用
        optionsBuilder.ConfigureWarnings(warnings =>
            warnings.Ignore(RelationalEventId.PendingModelChangesWarning));
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

    private void BeforeSaveChanges()
    {
        var entries = ChangeTracker.Entries()
            .Where(x => x.State is EntityState.Added or EntityState.Modified);
        foreach (var entry in entries)
        {
            var userContext = serviceProvider.GetService<IUserContext>();
            if (entry.State == EntityState.Added)
            {
                if (entry.Entity is ICreatable entity)
                {
                    entity.CreateTime = DateTime.Now;
                }

                if (entry.Entity is ICreatable<Guid?> creatable)
                {
                    creatable.CreatableId = userContext?.CurrentUserId;
                }
                else if (entry.Entity is ICreatable<Guid> creatableValue)
                {
                    creatableValue.CreatableId = userContext?.CurrentUserId ?? Guid.Empty;
                }
            }
            else
            {
                if (entry.Entity is IUpdatable entity)
                {
                    entity.UpdateTime = DateTime.Now;
                }

                if (entry.Entity is IUpdatable<Guid?> updatable)
                {
                    updatable.UpdatableId = userContext?.CurrentUserId;
                }
                else if (entry.Entity is IUpdatable<Guid> updatableValue)
                {
                    updatableValue.UpdatableId = userContext?.CurrentUserId ?? Guid.Empty;
                }
            }
        }
    }
}