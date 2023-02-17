// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Primitive;

namespace Snap.Hutao.Model.Metadata.Avatar;

/// <summary>
/// 料理奖励
/// </summary>
[HighQuality]
internal sealed class CookBonus
{
    /// <summary>
    /// 原型名称
    /// </summary>
    public MaterialId OriginItemId { get; set; } = default!;

    /// <summary>
    /// 名称
    /// </summary>
    public MaterialId ItemId { get; set; } = default!;

    /// <summary>
    /// 材料列表
    /// </summary>
    public List<MaterialId> InputList { get; set; } = default!;
}