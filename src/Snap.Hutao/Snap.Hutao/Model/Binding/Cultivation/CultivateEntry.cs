// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Binding.Gacha.Abstraction;

namespace Snap.Hutao.Model.Binding.Cultivation;

/// <summary>
/// 养成物品
/// </summary>
public class CultivateEntry : ItemBase
{
    /// <summary>
    /// Id
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// 入口Id
    /// </summary>
    public Guid EntryId { get; set; }

    /// <summary>
    /// 实体
    /// </summary>
    public List<CultivateItem> Items { get; set; } = default!;
}