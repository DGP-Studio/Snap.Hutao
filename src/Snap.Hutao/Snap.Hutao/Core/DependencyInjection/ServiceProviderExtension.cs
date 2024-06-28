// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Runtime.CompilerServices;

namespace Snap.Hutao.Core.DependencyInjection;

/// <summary>
/// 服务提供器扩展
/// </summary>
internal static class ServiceProviderExtension
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsDisposed(this IServiceProvider? serviceProvider, bool treatNullAsDisposed = true)
    {
        if (serviceProvider is null)
        {
            return treatNullAsDisposed;
        }

        try
        {
            _ = serviceProvider.GetRequiredService<IServiceScopeFactory>();
            return false;
        }
        catch (ObjectDisposedException)
        {
            return true;
        }
    }
}