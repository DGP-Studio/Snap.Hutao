﻿// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model;
using Snap.Hutao.Model.Intrinsic;

namespace Snap.Hutao.ViewModel.AvatarProperty;

/// <summary>
/// 装备基类
/// </summary>
[HighQuality]
internal abstract class EquipView : NameIconDescription
{
    /// <summary>
    /// 等级
    /// </summary>
    public string Level { get; set; } = default!;

    /// <summary>
    /// 品质
    /// </summary>
    public QualityType Quality { get; set; }

    /// <summary>
    /// 主属性
    /// </summary>
    public NameValue<string>? MainProperty { get; set; }

    internal EquipType EquipType { get; set; }
}
