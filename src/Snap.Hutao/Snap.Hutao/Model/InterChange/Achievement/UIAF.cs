// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Snap.Hutao.Model.InterChange.Achievement;

/// <summary>
/// 统一可交换成就格式
/// </summary>
public class UIAF
{
    /// <summary>
    /// 信息
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    public UIAFInfo Info { get; set; } = default!;

    /// <summary>
    /// 列表
    /// </summary>
    public List<UIAFItem> List { get; set; } = default!;
}