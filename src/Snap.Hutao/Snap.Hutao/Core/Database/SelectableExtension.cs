// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Runtime.CompilerServices;

namespace Snap.Hutao.Core.Database;

/// <summary>
/// 可枚举扩展
/// </summary>
[HighQuality]
internal static class SelectableExtension
{
    /// <summary>
    /// 获取选中的值或默认值
    /// </summary>
    /// <typeparam name="TSource">源类型</typeparam>
    /// <param name="source">源</param>
    /// <returns>选中的值或默认值</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TSource? SelectedOrDefault<TSource>(this IEnumerable<TSource> source)
        where TSource : ISelectable
    {
        return source.SingleOrDefault(i => i.IsSelected);
    }
}