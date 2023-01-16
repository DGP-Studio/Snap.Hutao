// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Extensions.DependencyInjection;

namespace Snap.Hutao.Core.DependencyInjection;

/// <summary>
/// 服务范围扩展
/// </summary>
public static class ServiceScopeExtension
{
    // Allow GC to Collect the IServiceScope
    private static readonly WeakReference<IServiceScope> ScopeReference = new(null!);

    /// <summary>
    /// 追踪服务范围
    /// </summary>
    /// <param name="scope">范围</param>
    public static void Track(this IServiceScope scope)
    {
        DisposeLast();
        ScopeReference.SetTarget(scope);
    }

    /// <summary>
    /// 释放上个范围
    /// </summary>
    public static void DisposeLast()
    {
        if (ScopeReference.TryGetTarget(out IServiceScope? scope))
        {
            scope.Dispose();
        }
    }
}