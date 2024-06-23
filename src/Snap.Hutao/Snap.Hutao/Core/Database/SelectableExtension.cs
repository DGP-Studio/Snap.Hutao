// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Database.Abstraction;
using System.Runtime.CompilerServices;

namespace Snap.Hutao.Core.Database;

/// <summary>
/// 可枚举扩展
/// </summary>
[HighQuality]
internal static class SelectableExtension
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TSource? SelectedOrDefault<TSource>(this IEnumerable<TSource> source)
        where TSource : ISelectable
    {
        return source.SingleOrDefault(i => i.IsSelected);
    }
}