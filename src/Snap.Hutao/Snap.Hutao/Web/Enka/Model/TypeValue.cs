// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Intrinsic;

namespace Snap.Hutao.Web.Enka.Model;

/// <summary>
/// 类型与值
/// </summary>
public class TypeValue
{
    /// <summary>
    /// 类型
    /// </summary>
    [JsonPropertyName("type")]
    public PlayerProperty Type { get; set; }

    /// <summary>
    /// 值
    /// </summary>
    [JsonPropertyName("val")]
    public string? Value { get; set; }
}