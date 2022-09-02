// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Intrinsic;

namespace Snap.Hutao.Model.Metadata;

/// <summary>
/// 属性信息
/// </summary>
public class PropertyInfo
{
    /// <summary>
    /// 提升的属性
    /// </summary>
    public IList<FightProperty> Properties { get; set; } = default!;

    /// <summary>
    /// 参数
    /// </summary>
    public IList<LevelParam<string, double>> Parameters { get; set; } = default!;
}
