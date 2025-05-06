// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Core.DependencyInjection;

internal static class ServiceProviderExtension
{
    public static IServiceScope CreateScope(this IServiceScopeFactory factory, bool deferDispose)
    {
        ArgumentNullException.ThrowIfNull(factory);
        return deferDispose
            ? new DeferDisposeServiceScope(DependencyInjection.DisposeDeferral(), factory.CreateScope())
            : factory.CreateScope();
    }

    public static IServiceScope CreateScope(this IServiceProvider provider, bool deferDispose)
    {
        ArgumentNullException.ThrowIfNull(provider);
        return deferDispose
            ? new DeferDisposeServiceScope(DependencyInjection.DisposeDeferral(), provider.CreateScope())
            : provider.CreateScope();
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

    private sealed partial class DeferDisposeServiceScope : IServiceScope
    {
        private readonly IDisposable deferToken;
        private readonly IServiceScope scope;

        public DeferDisposeServiceScope(IDisposable deferToken, IServiceScope scope)
        {
            this.deferToken = deferToken;
            this.scope = scope;
        }

        public IServiceProvider ServiceProvider => scope.ServiceProvider;

        public void Dispose()
        {
            scope.Dispose();
            deferToken.Dispose();
        }
    }
}