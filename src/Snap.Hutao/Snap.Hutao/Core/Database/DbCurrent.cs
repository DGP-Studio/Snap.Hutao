// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.Messaging;
using Microsoft.EntityFrameworkCore;

namespace Snap.Hutao.Core.Database;

/// <summary>
/// 数据库当前项
/// 简化对数据库中选中项的管理
/// </summary>
/// <typeparam name="TEntity">实体的类型</typeparam>
/// <typeparam name="TMessage">消息的类型</typeparam>
[HighQuality]
[Obsolete("Use ScopedDbCurrent instead")]
internal sealed class DbCurrent<TEntity, TMessage>
    where TEntity : class, ISelectable
    where TMessage : Message.ValueChangedMessage<TEntity>, new()
{
    private readonly DbSet<TEntity> dbSet;
    private readonly IMessenger messenger;

    private TEntity? current;

    /// <summary>
    /// 构造一个新的数据库当前项
    /// </summary>
    /// <param name="dbSet">数据集</param>
    /// <param name="messenger">消息器</param>
    public DbCurrent(DbSet<TEntity> dbSet, IMessenger messenger)
    {
        this.dbSet = dbSet;
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
            if (current?.InnerId == value?.InnerId)
            {
                return;
            }

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