// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Concurrent;

namespace Snap.Hutao.Core.DependencyInjection.Implementation.ServiceLookup;

internal sealed class CallSiteValidator : CallSiteVisitor<CallSiteValidator.CallSiteValidatorState, Type?>
{
    // Keys are services being resolved via GetService, values - first scoped service in their call site tree
    private readonly ConcurrentDictionary<ServiceCacheKey, Type?> scopedServices = [];

    public void ValidateCallSite(ServiceCallSite callSite)
    {
        VisitCallSite(callSite, default);
    }

    public void ValidateResolution(ServiceCallSite callSite, IServiceScope scope, IServiceScope rootScope)
    {
        if (ReferenceEquals(scope, rootScope)
            && scopedServices.TryGetValue(callSite.Cache.Key, out Type? scopedService)
            && scopedService != null)
        {
            Type serviceType = callSite.ServiceType;
            if (serviceType == scopedService)
            {
                throw new InvalidOperationException($"Cannot resolve {nameof(ServiceLifetime.Scoped).ToLowerInvariant()} service '{callSite.ServiceType}' from root provider.");
            }

            throw new InvalidOperationException($"Cannot resolve '{callSite.ServiceType}' from root provider because it requires {nameof(ServiceLifetime.Scoped).ToLowerInvariant()} service '{scopedService}'.");
        }
    }

    protected override Type? VisitCallSite(ServiceCallSite callSite, CallSiteValidatorState argument)
    {
        // First, check if we have encountered this call site before to prevent visiting call site trees that have already been visited
        // If firstScopedServiceInCallSiteTree is null there are no scoped dependencies in this service's call site tree
        // If firstScopedServiceInCallSiteTree has a value, it contains the first scoped service in this service's call site tree
        if (!scopedServices.TryGetValue(callSite.Cache.Key, out Type? firstScopedServiceInCallSiteTree))
        {
            // This call site wasn't cached yet, walk the tree
            firstScopedServiceInCallSiteTree = base.VisitCallSite(callSite, argument);

            // Cache the result
            scopedServices[callSite.Cache.Key] = firstScopedServiceInCallSiteTree;
        }

        // If there is a scoped service in the call site tree, make sure we are not resolving it from a singleton
        if (firstScopedServiceInCallSiteTree != null && argument.Singleton != null)
        {
            throw new InvalidOperationException($"Cannot consume {nameof(ServiceLifetime.Scoped).ToLowerInvariant()} service '{callSite.ServiceType}' from {nameof(ServiceLifetime.Singleton).ToLowerInvariant()} '{argument.Singleton.ServiceType}'.");
        }

        return firstScopedServiceInCallSiteTree;
    }

    protected override Type? VisitConstructor(ConstructorCallSite constructorCallSite, CallSiteValidatorState state)
    {
        Type? result = null;
        foreach (ServiceCallSite parameterCallSite in constructorCallSite.ParameterCallSites)
        {
            Type? scoped = VisitCallSite(parameterCallSite, state);
            result ??= scoped;
        }

        return result;
    }

    protected override Type? VisitIEnumerable(IEnumerableCallSite enumerableCallSite, CallSiteValidatorState state)
    {
        Type? result = null;
        foreach (ServiceCallSite serviceCallSite in enumerableCallSite.ServiceCallSites)
        {
            Type? scoped = VisitCallSite(serviceCallSite, state);
            result ??= scoped;
        }

        return result;
    }

    protected override Type? VisitRootCache(ServiceCallSite singletonCallSite, CallSiteValidatorState state)
    {
        state.Singleton = singletonCallSite;
        return VisitCallSiteMain(singletonCallSite, state);
    }

    protected override Type? VisitScopeCache(ServiceCallSite scopedCallSite, CallSiteValidatorState state)
    {
        // We are fine with having ServiceScopeService requested by singletons
        if (scopedCallSite.ServiceType == typeof(IServiceScopeFactory))
        {
            return null;
        }

        VisitCallSiteMain(scopedCallSite, state);
        return scopedCallSite.ServiceType;
    }

    protected override Type? VisitConstant(ConstantCallSite constantCallSite, CallSiteValidatorState state)
    {
        return null;
    }

    protected override Type? VisitServiceProvider(ServiceProviderCallSite serviceProviderCallSite, CallSiteValidatorState state)
    {
        return null;
    }

    protected override Type? VisitFactory(FactoryCallSite factoryCallSite, CallSiteValidatorState state)
    {
        return null;
    }

    internal struct CallSiteValidatorState
    {
        [DisallowNull]
        public ServiceCallSite? Singleton { get; set; }
    }
}