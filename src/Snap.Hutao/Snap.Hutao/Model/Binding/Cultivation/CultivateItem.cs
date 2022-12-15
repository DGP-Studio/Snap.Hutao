// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using Snap.Hutao.Model.Metadata;

namespace Snap.Hutao.Model.Binding.Cultivation;

/// <summary>
/// 养成物品
/// </summary>
public class CultivateItem : ObservableObject
{
    /// <summary>
    /// 养成物品
    /// </summary>
    /// <param name="inner">元数据</param>
    /// <param name="entity">实体</param>
    public CultivateItem(Material inner, Entity.CultivateItem entity)
    {
        Inner = inner;
        Entity = entity;
    }

    /// <summary>
    /// 元数据
    /// </summary>
    public Material Inner { get; }

    /// <summary>
    /// 实体
    /// </summary>
    public Entity.CultivateItem Entity { get; }
}