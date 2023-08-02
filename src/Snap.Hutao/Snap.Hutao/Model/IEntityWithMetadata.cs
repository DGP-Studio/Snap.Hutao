// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Model;

/// <summary>
/// 实体与元数据
/// </summary>
/// <typeparam name="TEntity">实体</typeparam>
/// <typeparam name="TMetadata">元数据</typeparam>
[HighQuality]
internal interface IEntityWithMetadata<out TEntity, out TMetadata> : IEntityOnly<TEntity>
{
    /// <summary>
    /// 元数据
    /// </summary>
    TMetadata Inner { get; }
}

internal interface IEntityOnly<out TEntity>
{
    /// <summary>
    /// 实体
    /// </summary>
    TEntity Entity { get; }
}