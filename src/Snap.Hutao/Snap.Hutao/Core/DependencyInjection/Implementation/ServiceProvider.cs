// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.DependencyInjection.Implementation.ServiceLookup;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Snap.Hutao.Core.DependencyInjection.Implementation;

/// <summary>
/// The default IServiceProvider.
/// </summary>
[DebuggerDisplay("{DebuggerToString(),nq}")]
[DebuggerTypeProxy(typeof(ServiceProviderDebugView))]
public sealed class ServiceProvider : IServiceProvider, IKeyedServiceProvider, IDisposable, IAsyncDisposable
{
    private readonly CallSiteValidator? callSiteValidator;
    private readonly Func<ServiceIdentifier, ServiceAccessor> createServiceAccessor;
    private readonly ConcurrentDictionary<ServiceIdentifier, ServiceAccessor> serviceAccessors;

    private ServiceProviderEngine engine;
    private bool disposed;

    internal ServiceProvider(ICollection<ServiceDescriptor> serviceDescriptors, ServiceProviderOptions options)
    {
        // note that Root needs to be set before calling GetEngine(), because the engine may need to access Root
        Root = new(this, isRootScope: true);
        engine = GetEngine();
        createServiceAccessor = CreateServiceAccessor;
        serviceAccessors = [];

        CallSiteFactory = new(serviceDescriptors);

        // The list of built-in services that aren't part of the list of service descriptors
        // keep this in sync with CallSiteFactory.IsService
        CallSiteFactory.Add(ServiceIdentifier.FromServiceType(typeof(IServiceProvider)), new ServiceProviderCallSite());
        CallSiteFactory.Add(ServiceIdentifier.FromServiceType(typeof(IServiceScopeFactory)), new ConstantCallSite(typeof(IServiceScopeFactory), Root));
        CallSiteFactory.Add(ServiceIdentifier.FromServiceType(typeof(IServiceProviderIsService)), new ConstantCallSite(typeof(IServiceProviderIsService), CallSiteFactory));
        CallSiteFactory.Add(ServiceIdentifier.FromServiceType(typeof(IServiceProviderIsKeyedService)), new ConstantCallSite(typeof(IServiceProviderIsKeyedService), CallSiteFactory));

        if (options.ValidateScopes)
        {
            callSiteValidator = new();
        }

        if (options.ValidateOnBuild)
        {
            List<Exception>? exceptions = null;
            foreach (ServiceDescriptor serviceDescriptor in serviceDescriptors)
            {
                try
                {
                    ValidateService(serviceDescriptor);
                }
                catch (Exception e)
                {
                    exceptions ??= [];
                    exceptions.Add(e);
                }
            }

            if (exceptions != null)
            {
                throw new AggregateException("Some services are not able to be constructed", exceptions.ToArray());
            }
        }
    }

    [FeatureSwitchDefinition("Microsoft.Extensions.DependencyInjection.VerifyOpenGenericServiceTrimmability")]
    internal static bool VerifyOpenGenericServiceTrimmability { get; } = AppContext.TryGetSwitch("Microsoft.Extensions.DependencyInjection.VerifyOpenGenericServiceTrimmability", out bool verifyOpenGenerics) && verifyOpenGenerics;

    [FeatureSwitchDefinition("Microsoft.Extensions.DependencyInjection.DisableDynamicEngine")]
    internal static bool DisableDynamicEngine { get; } = AppContext.TryGetSwitch("Microsoft.Extensions.DependencyInjection.DisableDynamicEngine", out bool disableDynamicEngine) && disableDynamicEngine;

    internal static bool VerifyAotCompatibility { get => !RuntimeFeature.IsDynamicCodeSupported; }

    internal CallSiteFactory CallSiteFactory { get; }

    internal ServiceProviderEngineScope Root { get; }

    /// <summary>
    /// Gets the service object of the specified type.
    /// </summary>
    /// <param name="serviceType">The type of the service to get.</param>
    /// <returns>The service that was produced.</returns>
    public object? GetService(Type serviceType)
    {
        return GetService(ServiceIdentifier.FromServiceType(serviceType), Root);
    }

    /// <summary>
    /// Gets the service object of the specified type with the specified key.
    /// </summary>
    /// <param name="serviceType">The type of the service to get.</param>
    /// <param name="serviceKey">The key of the service to get.</param>
    /// <returns>The keyed service.</returns>
    /// <exception cref="InvalidOperationException">The <see cref="KeyedService.AnyKey"/> value is used for <paramref name="serviceKey"/>
    /// when <paramref name="serviceType"/> is not an enumerable based on <see cref="IEnumerable{T}"/>.
    /// </exception>
    public object? GetKeyedService(Type serviceType, object? serviceKey)
    {
        return GetKeyedService(serviceType, serviceKey, Root);
    }

    /// <summary>
    /// Gets the service object of the specified type.
    /// </summary>
    /// <param name="serviceType">The type of the service to get.</param>
    /// <param name="serviceKey">The key of the service to get.</param>
    /// <returns>The keyed service.</returns>
    /// <exception cref="InvalidOperationException">The service wasn't found or the <see cref="KeyedService.AnyKey"/> value is used
    /// for <paramref name="serviceKey"/> when <paramref name="serviceType"/> is not an enumerable based on <see cref="IEnumerable{T}"/>.
    /// </exception>
    public object GetRequiredKeyedService(Type serviceType, object? serviceKey)
    {
        if (serviceKey == KeyedService.AnyKey)
        {
            if (!serviceType.IsGenericType || serviceType.GetGenericTypeDefinition() != typeof(IEnumerable<>))
            {
                ThrowHelper.ThrowInvalidOperationException_KeyedServiceAnyKeyUsedToResolveService();
            }
        }

        return GetRequiredKeyedService(serviceType, serviceKey, Root);
    }

    /// <inheritdoc />
    public void Dispose()
    {
        DisposeCore();
        Root.Dispose();
    }

    /// <inheritdoc/>
    public ValueTask DisposeAsync()
    {
        DisposeCore();
        return Root.DisposeAsync();
    }

    internal object? GetService(ServiceIdentifier serviceIdentifier, ServiceProviderEngineScope serviceProviderEngineScope)
    {
        if (disposed)
        {
            ThrowHelper.ThrowObjectDisposedException(true);
        }

        ServiceAccessor serviceAccessor = serviceAccessors.GetOrAdd(serviceIdentifier, createServiceAccessor);
        OnResolve(serviceAccessor.CallSite, serviceProviderEngineScope);
        object? result = serviceAccessor.RealizedService?.Invoke(serviceProviderEngineScope);
        Debug.Assert(result is null || CallSiteFactory.IsService(serviceIdentifier));
        return result;
    }

    internal object? GetKeyedService(Type serviceType, object? serviceKey, ServiceProviderEngineScope serviceProviderEngineScope)
    {
        if (serviceKey == KeyedService.AnyKey)
        {
            if (!serviceType.IsGenericType || serviceType.GetGenericTypeDefinition() != typeof(IEnumerable<>))
            {
                ThrowHelper.ThrowInvalidOperationException_KeyedServiceAnyKeyUsedToResolveService();
            }
        }

        return GetService(new(serviceKey, serviceType), serviceProviderEngineScope);
    }

    internal object GetRequiredKeyedService(Type serviceType, object? serviceKey, ServiceProviderEngineScope serviceProviderEngineScope)
    {
        object? service = GetKeyedService(serviceType, serviceKey, serviceProviderEngineScope);
        if (service == null)
        {
            if (serviceKey is null)
            {
                ThrowHelper.ThrowInvalidOperationException_NoServiceRegistered(serviceType);
            }
            else
            {
                ThrowHelper.ThrowInvalidOperationException_NoKeyedServiceRegistered(serviceType, serviceKey.GetType());
            }
        }

        return service;
    }

    internal IServiceScope CreateScope()
    {
        if (disposed)
        {
            ThrowHelper.ThrowObjectDisposedException(true);
        }

        return new ServiceProviderEngineScope(this, isRootScope: false);
    }

    internal bool IsDisposed()
    {
        return disposed;
    }

    internal void ReplaceServiceAccessor(ServiceCallSite callSite, Func<ServiceProviderEngineScope, object?> accessor)
    {
        serviceAccessors[new(callSite.Key, callSite.ServiceType)] = new()
        {
            CallSite = callSite,
            RealizedService = accessor,
        };
    }

    private void DisposeCore()
    {
        disposed = true;
    }

    private void OnCreate(ServiceCallSite callSite)
    {
        callSiteValidator?.ValidateCallSite(callSite);
    }

    private void OnResolve(ServiceCallSite? callSite, IServiceScope scope)
    {
        if (callSite is not null)
        {
            Debug.WriteLine($"Service: [{TypeNameHelper.GetTypeDisplayName(callSite.ServiceType)}@{(callSite.ImplementationType is null ? "<null>" : TypeNameHelper.GetTypeDisplayName(callSite.ImplementationType))}] resolved from {(((ServiceProviderEngineScope)scope).IsRootScope ? "Root" : "Scoped")}ServiceProvider.");
            callSiteValidator?.ValidateResolution(callSite, scope, Root);
        }
    }

    private void ValidateService(ServiceDescriptor descriptor)
    {
        if (descriptor.ServiceType is { IsGenericType: true, IsConstructedGenericType: false })
        {
            return;
        }

        try
        {
            ServiceCallSite? callSite = CallSiteFactory.GetCallSite(descriptor, new());
            if (callSite != null)
            {
                OnCreate(callSite);
            }
        }
        catch (Exception e)
        {
            throw new InvalidOperationException($"Error while validating the service descriptor '{descriptor}': {e.Message}", e);
        }
    }

    private ServiceAccessor CreateServiceAccessor(ServiceIdentifier serviceIdentifier)
    {
        ServiceCallSite? callSite = CallSiteFactory.GetCallSite(serviceIdentifier, new());
        if (callSite != null)
        {
            OnCreate(callSite);

            // Optimize singleton case
            if (callSite.Cache.Location == CallSiteResultCacheLocation.Root)
            {
                object? value = CallSiteRuntimeResolver.Instance.Resolve(callSite, Root);
                return new()
                {
                    CallSite = callSite,
                    RealizedService = scope => value,
                };
            }

            Func<ServiceProviderEngineScope, object?> realizedService = engine.RealizeService(callSite);
            return new()
            {
                CallSite = callSite,
                RealizedService = realizedService,
            };
        }

        return new()
        {
            CallSite = callSite,
            RealizedService = _ => null,
        };
    }

    private ServiceProviderEngine GetEngine()
    {
        ServiceProviderEngine engine;

        if (RuntimeFeature.IsDynamicCodeCompiled && !DisableDynamicEngine)
        {
            engine = CreateDynamicEngine();
        }
        else
        {
            // Don't try to compile Expressions/IL if they are going to get interpreted
            engine = RuntimeServiceProviderEngine.Instance;
        }

        return engine;

        [UnconditionalSuppressMessage("AotAnalysis", "IL3050:RequiresDynamicCode", Justification = "CreateDynamicEngine won't be called when using NativeAOT.")] // see also https://github.com/dotnet/linker/issues/2715
        ServiceProviderEngine CreateDynamicEngine()
        {
            return new DynamicServiceProviderEngine(this);
        }
    }

    private string DebuggerToString()
    {
        return Root.DebuggerToString();
    }

    internal sealed class ServiceProviderDebugView
    {
        private readonly ServiceProvider serviceProvider;

        public ServiceProviderDebugView(ServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public List<ServiceDescriptor> ServiceDescriptors { get => [.. serviceProvider.Root.RootProvider.CallSiteFactory.Descriptors]; }

        public List<object> Disposables { get => [.. serviceProvider.Root.Disposables]; }

        public bool Disposed { get => serviceProvider.Root.Disposed; }

        public bool IsScope { get => !serviceProvider.Root.IsRootScope; }
    }

    private sealed class ServiceAccessor
    {
        public ServiceCallSite? CallSite { get; set; }

        public Func<ServiceProviderEngineScope, object?>? RealizedService { get; set; }
    }
}