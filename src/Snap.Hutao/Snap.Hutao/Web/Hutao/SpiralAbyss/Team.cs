// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Json.Converter;

namespace Snap.Hutao.Web.Hutao.SpiralAbyss;

/// <summary>
/// 队伍
/// </summary>
[HighQuality]
internal sealed class Team
{
    /// <summary>
    /// 上半
    /// </summary>
    [JsonConverter(typeof(SeparatorCommaInt32EnumerableConverter))]
    public IEnumerable<int> UpHalf { get; set; } = default!;

    /// <summary>
    /// 下半
    /// </summary>
    [JsonConverter(typeof(SeparatorCommaInt32EnumerableConverter))]
    public IEnumerable<int> DownHalf { get; set; } = default!;
}