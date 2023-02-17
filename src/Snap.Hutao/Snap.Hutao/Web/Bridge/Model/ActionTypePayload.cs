// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Bridge.Model;

/// <summary>
/// 操作类型包装
/// </summary>
[HighQuality]
internal sealed class ActionTypePayload
{
    /// <summary>
    /// 操作类型
    /// </summary>
    [JsonPropertyName("action_type")]
    public string ActionType { get; set; } = default!;
}