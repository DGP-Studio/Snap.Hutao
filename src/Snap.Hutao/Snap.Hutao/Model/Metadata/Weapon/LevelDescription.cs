// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Model.Metadata.Weapon;

/// <summary>
/// 等级与描述
/// </summary>
/// <typeparam name="TLevel">等级的类型</typeparam>
public class LevelDescription<TLevel>
{
    /// <summary>
    /// 等级
    /// </summary>
    public TLevel Level { get; set; } = default!;

    /// <summary>
    /// 描述
    /// </summary>
    public string Description { get; set; } = default!;
}