// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Extensions.DependencyInjection;

namespace Snap.Hutao.Core.DependencyInjection;

/// <summary>
/// 服务范围扩展
/// </summary>
public static class ServiceScopeExtension
{
    private static IServiceScope? scopeReference;

    /// <summary>
    /// 追踪服务范围
    /// </summary>
    /// <param name="scope">范围</param>
    public static void Track(this IServiceScope scope)
    {
        DisposeLast();
        scopeReference = scope;
    }

    /// <summary>
    /// 释放上个范围
    /// </summary>
    public static void DisposeLast()
    {
        scopeReference?.Dispose();
    }
}
