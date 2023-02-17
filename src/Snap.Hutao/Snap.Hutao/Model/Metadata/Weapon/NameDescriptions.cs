// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Model.Metadata.Weapon;

/// <summary>
/// 武器被动信息
/// </summary>
[HighQuality]
internal sealed class NameDescriptions
{
    /// <summary>
    /// 被动的名称
    /// </summary>
    public string Name { get; set; } = default!;

    /// <summary>
    /// 各个等级的描述
    /// 0-4
    /// </summary>
    public List<LevelDescription> Descriptions { get; set; } = default!;
}