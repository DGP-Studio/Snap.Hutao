// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;

namespace Snap.Hutao.Web.Hutao.Model;

/// <summary>
/// 组队数据
/// </summary>
public class TeamCollocation
{
    /// <summary>
    /// 角色Id
    /// </summary>
    public int Avatar { get; set; }

    /// <summary>
    /// 角色搭配比率
    /// </summary>
    public IEnumerable<Rate<int>> Collocations { get; set; } = default!;
}
