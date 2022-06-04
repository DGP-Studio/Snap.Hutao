// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;

namespace Snap.Hutao.Web.Hutao.Model;

/// <summary>
/// 圣遗物配置数据
/// </summary>
public class AvatarReliquaryUsage
{
    /// <summary>
    /// 角色Id
    /// </summary>
    public int Avatar { get; set; }

    /// <summary>
    /// 圣遗物比率
    /// </summary>
    public IEnumerable<Rate<ReliquarySets>> ReliquaryUsage { get; set; } = default!;
}
