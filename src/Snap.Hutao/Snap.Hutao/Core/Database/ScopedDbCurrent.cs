// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.Messaging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Snap.Hutao.Core.Database;

/// <summary>
/// 范围化的数据库当前项
/// 简化对数据库中选中项的管理
/// </summary>
/// <typeparam name="TEntity">实体的类型</typeparam>
/// <typeparam name="TMessage">消息的类型</typeparam>
internal sealed class ScopedDbCurrent<TEntity, TMessage>
    where TEntity : class, ISelectable
    where TMessage : Message.ValueChangedMessage<TEntity>, new()
{
    private readonly IServiceScopeFactory scopeFactory;
    private readonly Func<IServiceProvider, DbSet<TEntity>> dbSetSelector;
    private readonly IMessenger messenger;

    private TEntity? current;

    /// <summary>
    /// 构造一个新的数据库当前项
    /// </summary>
    /// <param name="scopeFactory">范围工厂</param>
    /// <param name="dbSetSelector">数据集选择器</param>
    /// <param name="messenger">消息器</param>
    public ScopedDbCurrent(IServiceScopeFactory scopeFactory, Func<IServiceProvider, DbSet<TEntity>> dbSetSelector, IMessenger messenger)
    {
        this.scopeFactory = scopeFactory;
        this.dbSetSelector = dbSetSelector;
        this.messenger = messenger;
    }

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

            using (IServiceScope scope = scopeFactory.CreateScope())
            {
                DbSet<TEntity> dbSet = dbSetSelector(scope.ServiceProvider);

                // only update when not processing a deletion
                if (value != null)
                {
                    if (current != null)
                    {
                        current.IsSelected = false;
                        dbSet.UpdateAndSave(current);
                    }
                }

                TMessage message = new() { OldValue = current, NewValue = value };

                current = value;

                if (current != null)
                {
                    current.IsSelected = true;
                    dbSet.UpdateAndSave(current);
                }

                messenger.Send(message);
            }
        }
    }
}