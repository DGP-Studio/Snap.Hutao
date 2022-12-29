// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Metadata;

namespace Snap.Hutao.Model.Binding.Cultivation;

/// <summary>
/// 仅用于统计总数的养成物品
/// </summary>
public class StatisticsCultivateItem
{
    /// <summary>
    /// 构造一个新的统计用养成物品
    /// </summary>
    /// <param name="inner">材料</param>
    /// <param name="entity">实体</param>
    public StatisticsCultivateItem(Material inner, Entity.CultivateItem entity)
    {
        Inner = inner;
        Count = entity.Count;
    }

    /// <summary>
    /// 元数据
    /// </summary>
    public Material Inner { get; }

    /// <summary>
    /// 对应背包物品的个数
    /// </summary>
    public int Count { get; set; }

    /// <summary>
    /// 对应背包物品的个数
    /// </summary>
    public uint TotalCount { get; set; }

    /// <summary>
    /// 是否完成
    /// </summary>
    public bool IsFinished { get => Count >= TotalCount; }

    /// <summary>
    /// 格式化总数
    /// </summary>
    public string CountFormatted { get => $"{Count}/{TotalCount}"; }
}