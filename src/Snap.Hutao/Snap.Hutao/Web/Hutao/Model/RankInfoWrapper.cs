// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Web.Hutao.Model.Converter;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Snap.Hutao.Web.Hutao.Model;

/// <summary>
/// 排行包装
/// </summary>
public class RankInfoWrapper
{
    /// <summary>
    /// 伤害
    /// </summary>
    public RankInfo? Damage { get; set; }

    /// <summary>
    /// 承受伤害
    /// </summary>
    public RankInfo? TakeDamage { get; set; }
}
