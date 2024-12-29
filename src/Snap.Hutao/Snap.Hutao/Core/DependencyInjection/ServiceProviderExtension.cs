// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Runtime.CompilerServices;

namespace Snap.Hutao.Core.DependencyInjection;

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
    
    public static TService LockAndGetRequiredService<TService>(this IServiceProvider serviceProvider, Lock locker)
        where TService : notnull
    {
        ArgumentNullException.ThrowIfNull(serviceProvider);

        lock (locker)
        {
            return serviceProvider.GetRequiredService<TService>();
        }
    }
}