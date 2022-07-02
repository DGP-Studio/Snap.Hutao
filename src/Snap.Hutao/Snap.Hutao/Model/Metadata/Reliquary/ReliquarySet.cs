// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;

namespace Snap.Hutao.Model.Metadata.Reliquary;

/// <summary>
/// 圣遗物套装
/// </summary>
public class ReliquarySet
{
    /// <summary>
    /// 套装Id
    /// </summary>
    public int SetId { get; set; } = default!;

    /// <summary>
    /// 需要的数量
    /// </summary>
    public IEnumerable<int> NeedNumber { get; set; } = default!;

    /// <summary>
    /// 描述
    /// </summary>
    public IEnumerable<string> Descriptions { get; set; } = default!;
}