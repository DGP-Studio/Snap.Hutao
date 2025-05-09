// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Diagnostics;

namespace Snap.Hutao.Core.DependencyInjection.Implementation.ServiceLookup;

[DebuggerDisplay("{DebuggerToString(),nq}")]
[DebuggerTypeProxy(typeof(ServiceProviderEngineScopeDebugView))]
internal sealed partial class ServiceProviderEngineScope : IServiceScope, IServiceProvider, IKeyedServiceProvider, IAsyncDisposable, IServiceScopeFactory
{
    private bool disposed;
    private List<object>? disposables;

    public ServiceProviderEngineScope(ServiceProvider provider, bool isRootScope)
    {
        ResolvedServices = [];
        RootProvider = provider;
        IsRootScope = isRootScope;
    }

    public bool IsRootScope { get; }

    public IServiceProvider ServiceProvider => this;

    // For testing and debugging only
    internal IList<object> Disposables { get => disposables ?? []; }

    internal Dictionary<ServiceCacheKey, object?> ResolvedServices { get; }

    internal bool Disposed { get => disposed; }

    // This lock protects state on the scope, in particular, for the root scope, it protects
    // the list of disposable entries only, since ResolvedServices are cached on CallSites
    // For other scopes, it protects ResolvedServices and the list of disposables
    internal object Sync { get => ResolvedServices; }

    internal ServiceProvider RootProvider { get; }

    public object? GetService(Type serviceType)
    {
        if (disposed)
        {
            ThrowHelper.ThrowObjectDisposedException(IsRootScope);
        }

        return RootProvider.GetService(ServiceIdentifier.FromServiceType(serviceType), this);
    }

    public object? GetKeyedService(Type serviceType, object? serviceKey)
    {
        if (disposed)
        {
            ThrowHelper.ThrowObjectDisposedException(IsRootScope);
        }

        return RootProvider.GetKeyedService(serviceType, serviceKey, this);
    }

    public object GetRequiredKeyedService(Type serviceType, object? serviceKey)
    {
        if (disposed)
        {
            ThrowHelper.ThrowObjectDisposedException(IsRootScope);
        }

        return RootProvider.GetRequiredKeyedService(serviceType, serviceKey, this);
    }

    public IServiceScope CreateScope()
    {
        return RootProvider.CreateScope();
    }

    public void Dispose()
    {
        List<object>? toDispose = BeginDispose();

        if (toDispose is not null)
        {
            for (int i = toDispose.Count - 1; i >= 0; i--)
            {
                if (toDispose[i] is IDisposable disposable)
                {
                    disposable.Dispose();
                }
                else
                {
                    throw new InvalidOperationException($"'{TypeNameHelper.GetTypeDisplayName(toDispose[i])}' type only implements IAsyncDisposable. Use DisposeAsync to dispose the container.");
                }
            }
        }
    }

    public ValueTask DisposeAsync()
    {
        List<object>? toDispose = BeginDispose();

        if (toDispose != null)
        {
            try
            {
                for (int i = toDispose.Count - 1; i >= 0; i--)
                {
                    object disposable = toDispose[i];
                    if (disposable is IAsyncDisposable asyncDisposable)
                    {
                        ValueTask vt = asyncDisposable.DisposeAsync();
                        if (!vt.IsCompletedSuccessfully)
                        {
                            return Await(i, vt, toDispose);
                        }

                        // If its a IValueTaskSource backed ValueTask,
                        // inform it its result has been read so it can reset
                        vt.GetAwaiter().GetResult();
                    }
                    else
                    {
                        ((IDisposable)disposable).Dispose();
                    }
                }
            }
            catch (Exception ex)
            {
                return new(Task.FromException(ex));
            }
        }

        return default;

        static async ValueTask Await(int i, ValueTask vt, List<object> toDispose)
        {
            await vt.ConfigureAwait(false);

            // vt is acting on the disposable at index i,
            // decrement it and move to the next iteration
            i--;

            for (; i >= 0; i--)
            {
                object disposable = toDispose[i];
                if (disposable is IAsyncDisposable asyncDisposable)
                {
                    await asyncDisposable.DisposeAsync().ConfigureAwait(false);
                }
                else
                {
                    ((IDisposable)disposable).Dispose();
                }
            }
        }
    }

    [return: NotNullIfNotNull(nameof(service))]
    internal object? CaptureDisposable(object? service)
    {
        if (ReferenceEquals(this, service) || !(service is IDisposable || service is IAsyncDisposable))
        {
            return service;
        }

        bool disposed = false;
        lock (Sync)
        {
            if (this.disposed)
            {
                disposed = true;
            }
            else
            {
                disposables ??= [];

                disposables.Add(service);
            }
        }

        // Don't run customer code under the lock
        if (disposed)
        {
            if (service is IDisposable disposable)
            {
                disposable.Dispose();
            }
            else
            {
                // sync over async, for the rare case that an object only implements IAsyncDisposable and may end up starving the thread pool.
                object? localService = service; // copy to avoid closure on other paths
                Task.Run(() => ((IAsyncDisposable)localService).DisposeAsync().AsTask()).GetAwaiter().GetResult();
            }

            ThrowHelper.ThrowObjectDisposedException(IsRootScope);
        }

        return service;
    }

    internal string DebuggerToString()
    {
        string debugText = $"ServiceDescriptors = {RootProvider.CallSiteFactory.Descriptors.Length}";
        if (!IsRootScope)
        {
            debugText += $", IsScope = true";
        }

        if (disposed)
        {
            debugText += $", Disposed = true";
        }

        return debugText;
    }

    private List<object>? BeginDispose()
    {
        lock (Sync)
        {
            if (disposed)
            {
                return null;
            }

            // We've transitioned to the disposed state, so future calls to
            // CaptureDisposable will immediately dispose the object.
            // No further changes to _state.Disposables, are allowed.
            disposed = true;
        }

        if (IsRootScope && !RootProvider.IsDisposed())
        {
            // If this ServiceProviderEngineScope instance is a root scope, disposing this instance will need to dispose the RootProvider too.
            // Otherwise the RootProvider will never get disposed and will leak.
            // Note, if the RootProvider get disposed first, it will automatically dispose all attached ServiceProviderEngineScope objects.
            RootProvider.Dispose();
        }

        // ResolvedServices is never cleared for singletons because there might be a compilation running in background
        // trying to get a cached singleton service. If it doesn't find it
        // it will try to create a new one which will result in an ObjectDisposedException.
        return disposables;
    }

    private sealed class ServiceProviderEngineScopeDebugView
    {
        private readonly ServiceProviderEngineScope serviceProvider;

        public ServiceProviderEngineScopeDebugView(ServiceProviderEngineScope serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public List<ServiceDescriptor> ServiceDescriptors { get => [.. serviceProvider.RootProvider.CallSiteFactory.Descriptors]; }

        public List<object> Disposables { get => [.. serviceProvider.Disposables]; }

        public bool Disposed { get => serviceProvider.disposed; }

        public bool IsScope { get => !serviceProvider.IsRootScope; }
    }
}