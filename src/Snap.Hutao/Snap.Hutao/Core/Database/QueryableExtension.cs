// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace Snap.Hutao.Core.Database;

/// <summary>
/// 可查询扩展
/// </summary>
[HighQuality]
internal static class QueryableExtension
{
    /// <summary>
    /// source.Where(predicate).ExecuteDelete()
    /// </summary>
    /// <typeparam name="TSource">源类型</typeparam>
    /// <param name="source">源</param>
    /// <param name="predicate">条件</param>
    /// <returns>SQL返回个数</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int ExecuteDeleteWhere<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate)
    {
        return source.Where(predicate).ExecuteDelete();
    }

    /// <summary>
    /// source.Where(predicate).ExecuteDeleteAsync(token)
    /// </summary>
    /// <typeparam name="TSource">源类型</typeparam>
    /// <param name="source">源</param>
    /// <param name="predicate">条件</param>
    /// <param name="token">取消令牌</param>
    /// <returns>SQL返回个数</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ValueTask<int> ExecuteDeleteWhereAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate, CancellationToken token = default)
    {
        return source.Where(predicate).ExecuteDeleteAsync(token).AsValueTask();
    }
}
