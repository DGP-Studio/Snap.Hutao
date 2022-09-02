// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Json.Converter;

namespace Snap.Hutao.Web.Hutao.Model;

/// <summary>
/// 队伍
/// </summary>
public class Team
{
    /// <summary>
    /// 上半
    /// </summary>
    [JsonConverter(typeof(SeparatorCommaInt32EnumerableConverter))]
    public IEnumerable<int> UpHalf { get; set; } = null!;

    /// <summary>
    /// 下半
    /// </summary>
    [JsonConverter(typeof(SeparatorCommaInt32EnumerableConverter))]
    public IEnumerable<int> DownHalf { get; set; } = null!;
}