// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Snap.Hutao.Model.Metadata;

namespace Snap.Hutao.Model.Binding.Cultivation;

/// <summary>
/// 养成物品
/// </summary>
public class CultivateItem : ObservableObject
{
    private bool isFinished;

    /// <summary>
    /// 养成物品
    /// </summary>
    /// <param name="inner">元数据</param>
    /// <param name="entity">实体</param>
    public CultivateItem(Material inner, Entity.CultivateItem entity)
    {
        Inner = inner;
        Entity = entity;
        isFinished = Entity.IsFinished;
    }

    /// <summary>
    /// 元数据
    /// </summary>
    public Material Inner { get; }

    /// <summary>
    /// 实体
    /// </summary>
    public Entity.CultivateItem Entity { get; }

    /// <summary>
    /// 是否完成此项
    /// </summary>
    public bool IsFinished
    {
        get => isFinished; set
        {
            if (SetProperty(ref isFinished, value))
            {
                Entity.IsFinished = value;
            }
        }
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