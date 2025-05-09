// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Core.DependencyInjection;

internal static class ServiceProviderExtension
{
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