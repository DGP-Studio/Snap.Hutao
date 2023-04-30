// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Intrinsic;

namespace Snap.Hutao.Model.Metadata;

/// <summary>
/// 属性信息
/// </summary>
[HighQuality]
internal sealed class PropertiesParameters
{
    /// <summary>
    /// 提升的属性
    /// </summary>
    public List<FightProperty> Properties { get; set; } = default!;

    /// <summary>
    /// 参数
    /// </summary>
    public List<LevelParameters<string, float>> Parameters { get; set; } = default!;
}
