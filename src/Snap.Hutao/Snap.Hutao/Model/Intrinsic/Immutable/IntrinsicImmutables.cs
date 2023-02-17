// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Concurrent;
using System.Globalization;

namespace Snap.Hutao.Model.Intrinsic.Immutable;

/// <summary>
/// 不可变的原生枚举
/// </summary>
[HighQuality]
internal static class IntrinsicImmutables
{
    private static readonly ConcurrentDictionary<string, LocalizedIntrinsicImmutables> EnumMap = new();

    /// <summary>
    /// 获取当前语言的枚举
    /// </summary>
    /// <param name="culture">指定的语言</param>
    /// <returns>枚举</returns>
    public static LocalizedIntrinsicImmutables GetForCurrentCulture(CultureInfo? culture = default)
    {
        culture ??= CultureInfo.CurrentCulture;
        return EnumMap.GetOrAdd(culture.Name, (name) => new());
    }
}