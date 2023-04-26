// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using Snap.Hutao.Model;
using Snap.Hutao.Model.Metadata.Item;

namespace Snap.Hutao.ViewModel.Cultivation;

/// <summary>
/// 养成物品
/// </summary>
[HighQuality]
internal sealed class CultivateItemView : ObservableObject, IEntityWithMetadata<Model.Entity.CultivateItem, Material>
{
    /// <summary>
    /// 养成物品
    /// </summary>
    /// <param name="entity">实体</param>
    /// <param name="inner">元数据</param>
    public CultivateItemView(Model.Entity.CultivateItem entity, Material inner)
    {
        Entity = entity;
        Inner = inner;
    }

    /// <summary>
    /// 元数据
    /// </summary>
    public Material Inner { get; }

    /// <summary>
    /// 实体
    /// </summary>
    public Model.Entity.CultivateItem Entity { get; }

    /// <summary>
    /// 是否完成此项
    /// </summary>
    public bool IsFinished
    {
        get => Entity.IsFinished;
        set => SetProperty(Entity.IsFinished, value, Entity, (entity, isFinished) => entity.IsFinished = isFinished);
    }

    /// <summary>
    /// 是否为今日物品
    /// </summary>
    public bool IsToday { get => Inner.IsTodaysItem(); }

    /// <summary>
    /// 星期中的日期
    /// </summary>
    public DaysOfWeek DaysOfWeek { get => Inner.GetDaysOfWeek(); }
}