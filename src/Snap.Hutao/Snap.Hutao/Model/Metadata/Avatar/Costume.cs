// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Primitive;

namespace Snap.Hutao.Model.Metadata.Avatar;

/// <summary>
/// 皮肤
/// </summary>
[HighQuality]
internal sealed class Costume
{
    /// <summary>
    /// Id
    /// </summary>
    public CostumeId Id { get; set; }

    /// <summary>
    /// 名称
    /// </summary>
    public string Name { get; set; } = default!;

    /// <summary>
    /// 描述
    /// </summary>
    public string Description { get; set; } = default!;

    /// <summary>
    /// 是否为默认
    /// </summary>
    public bool IsDefault { get; set; }

    /// <summary>
    /// 图标
    /// </summary>
    public string Icon { get; set; } = default!;

    /// <summary>
    /// 侧面图标
    /// </summary>
    public string SideIcon { get; set; } = default!;
}