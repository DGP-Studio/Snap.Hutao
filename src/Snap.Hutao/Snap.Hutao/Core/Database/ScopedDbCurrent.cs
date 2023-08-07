// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.Messaging;
using Microsoft.EntityFrameworkCore;
using Snap.Hutao.Model;
using Snap.Hutao.Model.Entity.Database;

namespace Snap.Hutao.Core.Database;

/// <summary>
/// 范围化的数据库当前项
/// 简化对数据库中选中项的管理
/// </summary>
/// <typeparam name="TEntity">实体的类型</typeparam>
/// <typeparam name="TMessage">消息的类型</typeparam>
[ConstructorGenerated]
internal sealed partial class ScopedDbCurrent<TEntity, TMessage>
    where TEntity : class, ISelectable
    where TMessage : Message.ValueChangedMessage<TEntity>, new()
{
    private readonly IServiceProvider serviceProvider;
    private readonly IMessenger messenger;

    private TEntity? current;

    /// <summary>
    /// 当前选中的项
    /// </summary>
    public TEntity? Current
    {
        get => current;
        set
        {
            // prevent useless sets
            if (current == value)
            {
                return;
            }

            // TODO: Troubeshooting why the serviceProvider will NRE
            using (IServiceScope scope = serviceProvider.CreateScope())
            {
                AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                DbSet<TEntity> dbSet = appDbContext.Set<TEntity>();

                // only update when not processing a deletion
                if (value is not null)
                {
                    if (current is not null)
                    {
                        current.IsSelected = false;
                        dbSet.UpdateAndSave(current);
                    }
                }

                TMessage message = new() { OldValue = current, NewValue = value };

                current = value;

                if (current is not null)
                {
                    current.IsSelected = true;
                    dbSet.UpdateAndSave(current);
                }

                messenger.Send(message);
            }
        }
    }
}

[ConstructorGenerated]
internal sealed partial class ScopedDbCurrent<TEntityOnly, TEntity, TMessage>
    where TEntityOnly : class, IEntityOnly<TEntity>
    where TEntity : class, ISelectable
    where TMessage : Message.ValueChangedMessage<TEntityOnly>, new()
{
    private readonly IServiceProvider serviceProvider;
    private readonly IMessenger messenger;

    private TEntityOnly? current;

    /// <summary>
    /// 当前选中的项
    /// </summary>
    public TEntityOnly? Current
    {
        get => current;
        set
        {
            // prevent useless sets
            if (current == value)
            {
                return;
            }

            // TODO: Troubeshooting why the serviceProvider will NRE
            using (IServiceScope scope = serviceProvider.CreateScope())
            {
                AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                DbSet<TEntity> dbSet = appDbContext.Set<TEntity>();

                // only update when not processing a deletion
                if (value is not null)
                {
                    if (current is not null)
                    {
                        current.Entity.IsSelected = false;
                        dbSet.UpdateAndSave(current.Entity);
                    }
                }

                TMessage message = new() { OldValue = current, NewValue = value };

                current = value;

                if (current is not null)
                {
                    current.Entity.IsSelected = true;
                    dbSet.UpdateAndSave(current.Entity);
                }

                messenger.Send(message);
            }
        }
    }
}