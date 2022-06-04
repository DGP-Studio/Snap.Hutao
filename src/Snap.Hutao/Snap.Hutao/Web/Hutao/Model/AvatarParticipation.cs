// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;

namespace Snap.Hutao.Web.Hutao.Model;

/// <summary>
/// 出场数据
/// </summary>
public class AvatarParticipation
{
    /// <summary>
    /// 层
    /// </summary>
    public int Floor { get; set; }

    /// <summary>
    /// 角色比率
    /// </summary>
    public IEnumerable<Rate<int>> AvatarUsage { get; set; } = default!;
}
