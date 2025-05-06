// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Runtime.CompilerServices;

namespace Snap.Hutao.Core.DependencyInjection;

internal static class ServiceProviderExtension
{
    public static IServiceScope CreateScope(this IServiceProvider provider, IIsDisposed isDisposed)
    {
        isDisposed.TryThrow();
        return provider.CreateScope();
    }

    public static IServiceScope CreateScope(this IServiceScopeFactory factory, IIsDisposed isDisposed)
    {
        isDisposed.TryThrow();
        return factory.CreateScope();
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