// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Model.Binding.AvatarProperty;

/// <summary>
/// 圣遗物
/// </summary>
public class Reliquary : EquipBase
{
    /// <summary>
    /// 副属性列表
    /// </summary>
    public List<Pair<string, string>> SubProperties { get; set; } = default!;
}
