// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Concurrent;
using System.Diagnostics;
using System.Reflection;

namespace Snap.Hutao.Core.DependencyInjection.Implementation.ServiceLookup;

internal sealed class CallSiteFactory : IServiceProviderIsService, IServiceProviderIsKeyedService
{
    private const int DefaultSlot = 0;
    private readonly ConcurrentDictionary<ServiceCacheKey, ServiceCallSite> callSiteCache = [];
    private readonly Dictionary<ServiceIdentifier, ServiceDescriptorCacheItem> descriptorLookup = [];
    private readonly ConcurrentDictionary<ServiceIdentifier, object> callSiteLocks = [];

    private readonly StackGuard stackGuard;

    public CallSiteFactory(ICollection<ServiceDescriptor> descriptors)
    {
        stackGuard = new();
        Descriptors = new ServiceDescriptor[descriptors.Count];
        descriptors.CopyTo(Descriptors, 0);

        Populate();
    }

    internal ServiceDescriptor[] Descriptors { get; }

    public void Add(ServiceIdentifier serviceIdentifier, ServiceCallSite serviceCallSite)
    {
        callSiteCache[new(serviceIdentifier, DefaultSlot)] = serviceCallSite;
    }

    public bool IsService(Type serviceType)
    {
        return IsService(new ServiceIdentifier(null, serviceType));
    }

    public bool IsKeyedService(Type serviceType, object? key)
    {
        return IsService(new ServiceIdentifier(key, serviceType));
    }

    // For unit testing
    internal int? GetSlot(ServiceDescriptor serviceDescriptor)
    {
        if (descriptorLookup.TryGetValue(ServiceIdentifier.FromDescriptor(serviceDescriptor), out ServiceDescriptorCacheItem item))
        {
            return item.GetSlot(serviceDescriptor);
        }

        return null;
    }

    internal ServiceCallSite? GetCallSite(ServiceIdentifier serviceIdentifier, CallSiteChain callSiteChain)
    {
        return callSiteCache.TryGetValue(new(serviceIdentifier, DefaultSlot), out ServiceCallSite? site)
            ? site
            : CreateCallSite(serviceIdentifier, callSiteChain);
    }

    internal ServiceCallSite? GetCallSite(ServiceDescriptor serviceDescriptor, CallSiteChain callSiteChain)
    {
        ServiceIdentifier serviceIdentifier = ServiceIdentifier.FromDescriptor(serviceDescriptor);
        if (descriptorLookup.TryGetValue(serviceIdentifier, out ServiceDescriptorCacheItem descriptor))
        {
            return TryCreateExact(serviceDescriptor, serviceIdentifier, callSiteChain, descriptor.GetSlot(serviceDescriptor));
        }

        Debug.Fail("_descriptorLookup didn't contain requested serviceDescriptor");
        return null;
    }

    internal bool IsService(ServiceIdentifier serviceIdentifier)
    {
        Type serviceType = serviceIdentifier.ServiceType;

        ArgumentNullException.ThrowIfNull(serviceType);

        // Querying for an open generic should return false (they aren't resolvable)
        if (serviceType.IsGenericTypeDefinition)
        {
            return false;
        }

        if (descriptorLookup.ContainsKey(serviceIdentifier))
        {
            return true;
        }

        if (serviceIdentifier.ServiceKey != null && descriptorLookup.ContainsKey(new ServiceIdentifier(KeyedService.AnyKey, serviceType)))
        {
            return true;
        }

        if (serviceType.IsConstructedGenericType && serviceType.GetGenericTypeDefinition() is { } genericDefinition)
        {
            // We special case IEnumerable since it isn't explicitly registered in the container,
            // yet we can manifest instances of it when requested.
            return genericDefinition == typeof(IEnumerable<>) || descriptorLookup.ContainsKey(serviceIdentifier.GetGenericTypeDefinition());
        }

        // These are the built-in service types that aren't part of the list of service descriptors
        // If you update these make sure to also update the code in ServiceProvider.ctor
        return serviceType == typeof(IServiceProvider) ||
            serviceType == typeof(IServiceScopeFactory) ||
            serviceType == typeof(IServiceProviderIsService) ||
            serviceType == typeof(IServiceProviderIsKeyedService);
    }

    /// <summary>
    /// Validates that two generic type definitions have compatible trimming annotations on their generic arguments.
    /// </summary>
    /// <remarks>
    /// When open generic types are used in DI, there is an error when the concrete implementation type
    /// has [DynamicallyAccessedMembers] attributes on a generic argument type, but the interface/service type
    /// doesn't have matching annotations. The problem is that the trimmer doesn't see the members that need to
    /// be preserved on the type being passed to the generic argument. But when the interface/service type also has
    /// the annotations, the trimmer will see which members need to be preserved on the closed generic argument type.
    /// </remarks>
    private static void ValidateTrimmingAnnotations(
        Type serviceType,
        Type[] serviceTypeGenericArguments,
        Type implementationType,
        Type[] implementationTypeGenericArguments)
    {
        Debug.Assert(serviceTypeGenericArguments.Length == implementationTypeGenericArguments.Length);

        for (int i = 0; i < serviceTypeGenericArguments.Length; i++)
        {
            Type serviceGenericType = serviceTypeGenericArguments[i];
            Type implementationGenericType = implementationTypeGenericArguments[i];

            DynamicallyAccessedMemberTypes serviceDynamicallyAccessedMembers = GetDynamicallyAccessedMemberTypes(serviceGenericType);
            DynamicallyAccessedMemberTypes implementationDynamicallyAccessedMembers = GetDynamicallyAccessedMemberTypes(implementationGenericType);

            if (!AreCompatible(serviceDynamicallyAccessedMembers, implementationDynamicallyAccessedMembers))
            {
                throw new ArgumentException($"Generic implementation type '{implementationType.FullName}' has a DynamicallyAccessedMembers attribute applied to a generic argument type, but the service type '{serviceType.FullName}' doesn't have a matching DynamicallyAccessedMembers attribute on its generic argument type.");
            }

            bool serviceHasNewConstraint = serviceGenericType.GenericParameterAttributes.HasFlag(GenericParameterAttributes.DefaultConstructorConstraint);
            bool implementationHasNewConstraint = implementationGenericType.GenericParameterAttributes.HasFlag(GenericParameterAttributes.DefaultConstructorConstraint);
            if (implementationHasNewConstraint && !serviceHasNewConstraint)
            {
                throw new ArgumentException($"Generic implementation type '{implementationType.FullName}' has a DefaultConstructorConstraint ('new()' constraint), but the generic service type '{serviceType.FullName}' doesn't.");
            }
        }
    }

    private static DynamicallyAccessedMemberTypes GetDynamicallyAccessedMemberTypes(Type serviceGenericType)
    {
        foreach (CustomAttributeData attributeData in serviceGenericType.GetCustomAttributesData())
        {
            if (attributeData.AttributeType.FullName == "System.Diagnostics.CodeAnalysis.DynamicallyAccessedMembersAttribute" &&
                attributeData.ConstructorArguments is [{ ArgumentType.FullName: "System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes" }])
            {
                return (DynamicallyAccessedMemberTypes)(int)attributeData.ConstructorArguments[0].Value!;
            }
        }

        return DynamicallyAccessedMemberTypes.None;
    }

    private static bool AreCompatible(DynamicallyAccessedMemberTypes serviceDynamicallyAccessedMembers, DynamicallyAccessedMemberTypes implementationDynamicallyAccessedMembers)
    {
        // The DynamicallyAccessedMemberTypes don't need to exactly match.
        // The service type needs to preserve a superset of the members required by the implementation type.
        return serviceDynamicallyAccessedMembers.HasFlag(implementationDynamicallyAccessedMembers);
    }

    private static CallSiteResultCacheLocation GetCommonCacheLocation(CallSiteResultCacheLocation locationA, CallSiteResultCacheLocation locationB)
    {
        return (CallSiteResultCacheLocation)Math.Max((int)locationA, (int)locationB);
    }

    private static bool ShouldCreateExact(Type descriptorType, Type serviceType)
    {
        return descriptorType == serviceType;
    }

    private static bool ShouldCreateOpenGeneric(Type descriptorType, Type serviceType)
    {
        return serviceType.IsConstructedGenericType && serviceType.GetGenericTypeDefinition() == descriptorType;
    }

    /// <summary>
    /// Verifies none of the generic type arguments are ValueTypes.
    /// </summary>
    /// <remarks>
    /// NativeAOT apps are not guaranteed that the native code for the closed generic of ValueType
    /// has been generated. To catch these problems early, this verification is enabled at development-time
    /// to inform the developer early that this scenario will not work once AOT'd.
    /// </remarks>
    private static void VerifyOpenGenericAotCompatibility(Type serviceType, Type[] genericTypeArguments)
    {
        foreach (Type typeArg in genericTypeArguments)
        {
            if (typeArg.IsValueType)
            {
                throw new InvalidOperationException($"Unable to create a generic service for type '{serviceType}' because '{typeArg}' is a ValueType. Native code to support creating generic services might not be available with native AOT.");
            }
        }
    }

    private void Populate()
    {
        foreach (ServiceDescriptor descriptor in Descriptors)
        {
            Type serviceType = descriptor.ServiceType;
            if (serviceType.IsGenericTypeDefinition)
            {
                Type? implementationType = descriptor.GetImplementationType();

                if (implementationType is not { IsGenericTypeDefinition: true })
                {
                    throw new ArgumentException($"Open generic service type '{serviceType}' requires registering an open generic implementation type.", "descriptors");
                }

                if (implementationType.IsAbstract || implementationType.IsInterface)
                {
                    throw new ArgumentException($"Cannot instantiate implementation type '{implementationType}' for service type '{serviceType}'.");
                }

                Type[] serviceTypeGenericArguments = serviceType.GetGenericArguments();
                Type[] implementationTypeGenericArguments = implementationType.GetGenericArguments();
                if (serviceTypeGenericArguments.Length != implementationTypeGenericArguments.Length)
                {
                    throw new ArgumentException($"Arity of open generic service type '{serviceType}' does not equal arity of open generic implementation type '{implementationType}'.", "descriptors");
                }

                if (ServiceProvider.VerifyOpenGenericServiceTrimmability)
                {
                    ValidateTrimmingAnnotations(serviceType, serviceTypeGenericArguments, implementationType, implementationTypeGenericArguments);
                }
            }
            else if (descriptor.TryGetImplementationType(out Type? implementationType))
            {
                Debug.Assert(implementationType != null);

                if (implementationType.IsGenericTypeDefinition ||
                    implementationType.IsAbstract ||
                    implementationType.IsInterface)
                {
                    throw new ArgumentException($"Cannot instantiate implementation type '{implementationType}' for service type '{serviceType}'.");
                }
            }

            ServiceIdentifier cacheKey = ServiceIdentifier.FromDescriptor(descriptor);
            descriptorLookup.TryGetValue(cacheKey, out ServiceDescriptorCacheItem cacheItem);
            descriptorLookup[cacheKey] = cacheItem.Add(descriptor);
        }
    }

    private ServiceCallSite? CreateCallSite(ServiceIdentifier serviceIdentifier, CallSiteChain callSiteChain)
    {
        if (!stackGuard.TryEnterOnCurrentStack())
        {
            return stackGuard.RunOnEmptyStack(CreateCallSite, serviceIdentifier, callSiteChain);
        }

        // We need to lock the resolution process for a single service type at a time:
        // Consider the following:
        // C -> D -> A
        // E -> D -> A
        // Resolving C and E in parallel means that they will be modifying the callsite cache concurrently
        // to add the entry for C and E, but the resolution of D and A is synchronized
        // to make sure C and E both reference the same instance of the callsite.
        //
        // This is to make sure we can safely store singleton values on the callsites themselves
        object callsiteLock = callSiteLocks.GetOrAdd(serviceIdentifier, static _ => new());

        lock (callsiteLock)
        {
            callSiteChain.CheckCircularDependency(serviceIdentifier);

            ServiceCallSite? callSite = TryCreateExact(serviceIdentifier, callSiteChain)
                ?? TryCreateOpenGeneric(serviceIdentifier, callSiteChain)
                ?? TryCreateEnumerable(serviceIdentifier, callSiteChain);

            return callSite;
        }
    }

    private ServiceCallSite? TryCreateExact(ServiceIdentifier serviceIdentifier, CallSiteChain callSiteChain)
    {
        if (descriptorLookup.TryGetValue(serviceIdentifier, out ServiceDescriptorCacheItem descriptor))
        {
            return TryCreateExact(descriptor.Last, serviceIdentifier, callSiteChain, DefaultSlot);
        }

        if (serviceIdentifier.ServiceKey != null)
        {
            // Check if there is a registration with KeyedService.AnyKey
            ServiceIdentifier catchAllIdentifier = new(KeyedService.AnyKey, serviceIdentifier.ServiceType);
            if (descriptorLookup.TryGetValue(catchAllIdentifier, out descriptor))
            {
                return TryCreateExact(descriptor.Last, serviceIdentifier, callSiteChain, DefaultSlot);
            }
        }

        return null;
    }

    private ServiceCallSite? TryCreateOpenGeneric(ServiceIdentifier serviceIdentifier, CallSiteChain callSiteChain)
    {
        if (serviceIdentifier.ServiceType.IsConstructedGenericType)
        {
            ServiceIdentifier genericIdentifier = serviceIdentifier.GetGenericTypeDefinition();
            if (descriptorLookup.TryGetValue(genericIdentifier, out ServiceDescriptorCacheItem descriptor))
            {
                return TryCreateOpenGeneric(descriptor.Last, serviceIdentifier, callSiteChain, DefaultSlot, true);
            }

            if (serviceIdentifier.ServiceKey != null)
            {
                // Check if there is a registration with KeyedService.AnyKey
                ServiceIdentifier catchAllIdentifier = new(KeyedService.AnyKey, genericIdentifier.ServiceType);
                if (descriptorLookup.TryGetValue(catchAllIdentifier, out descriptor))
                {
                    return TryCreateOpenGeneric(descriptor.Last, serviceIdentifier, callSiteChain, DefaultSlot, true);
                }
            }
        }

        return null;
    }

    private ServiceCallSite? TryCreateEnumerable(ServiceIdentifier serviceIdentifier, CallSiteChain callSiteChain)
    {
        ServiceCacheKey callSiteKey = new(serviceIdentifier, DefaultSlot);
        if (callSiteCache.TryGetValue(callSiteKey, out ServiceCallSite? serviceCallSite))
        {
            return serviceCallSite;
        }

        try
        {
            callSiteChain.Add(serviceIdentifier);

            Type serviceType = serviceIdentifier.ServiceType;

            if (!serviceType.IsConstructedGenericType ||
                serviceType.GetGenericTypeDefinition() != typeof(IEnumerable<>))
            {
                return null;
            }

            Type itemType = serviceType.GenericTypeArguments[0];
            ServiceIdentifier cacheKey = new(serviceIdentifier.ServiceKey, itemType);
            if (ServiceProvider.VerifyAotCompatibility && itemType.IsValueType)
            {
                // NativeAOT apps are not able to make Enumerable of ValueType services
                // since there is no guarantee the ValueType[] code has been generated.
                throw new InvalidOperationException($"Unable to create an Enumerable service of type '{itemType}' because it is a ValueType. Native code to support creating Enumerable services might not be available with native AOT.");
            }

            CallSiteResultCacheLocation cacheLocation = CallSiteResultCacheLocation.Root;
            ServiceCallSite[] callSites;

            bool isAnyKeyLookup = serviceIdentifier.ServiceKey == KeyedService.AnyKey;

            // If item type is not generic we can safely use descriptor cache
            // Special case for KeyedService.AnyKey, we don't want to check the cache because a KeyedService.AnyKey registration
            // will "hide" all the other service registration
            if (!itemType.IsConstructedGenericType &&
                !isAnyKeyLookup &&
                descriptorLookup.TryGetValue(cacheKey, out ServiceDescriptorCacheItem descriptors))
            {
                // Last service will get slot 0.
                int slot = descriptors.Count;

                callSites = new ServiceCallSite[descriptors.Count];
                for (int i = 0; i < descriptors.Count; i++)
                {
                    ServiceDescriptor descriptor = descriptors[i];

                    // There are no open generics here, so we only need to call CreateExact().
                    ServiceCallSite callSite = CreateExact(descriptor, cacheKey, callSiteChain, --slot);
                    cacheLocation = GetCommonCacheLocation(cacheLocation, callSite.Cache.Location);
                    callSites[i] = callSite;
                }
            }
            else
            {
                // We need to construct a list of matching call sites in declaration order, but to ensure
                // correct caching we must assign slots in reverse declaration order and with slots being
                // given out first to any exact matches before any open generic matches. Therefore, we
                // iterate over the descriptors twice in reverse, catching exact matches on the first pass
                // and open generic matches on the second pass.
                List<KeyValuePair<int, ServiceCallSite>> callSitesByIndex = [];
                Dictionary<ServiceIdentifier, int>? keyedSlotAssignment = null;
                int slot = 0;

                // Do the exact matches first.
                for (int i = this.Descriptors.Length - 1; i >= 0; i--)
                {
                    if (KeysMatch(cacheKey.ServiceKey, this.Descriptors[i].ServiceKey))
                    {
                        if (ShouldCreateExact(this.Descriptors[i].ServiceType, cacheKey.ServiceType))
                        {
                            // For AnyKey, we want to cache based on descriptor identity, not AnyKey that cacheKey has.
                            ServiceIdentifier registrationKey = isAnyKeyLookup ? ServiceIdentifier.FromDescriptor(this.Descriptors[i]) : cacheKey;
                            slot = GetSlot(registrationKey);
                            ServiceCallSite callSite = CreateExact(this.Descriptors[i], registrationKey, callSiteChain, slot);
                            AddCallSite(callSite, i);
                            UpdateSlot(registrationKey);
                        }
                    }
                }

                // Do the open generic matches second.
                for (int i = this.Descriptors.Length - 1; i >= 0; i--)
                {
                    if (KeysMatch(cacheKey.ServiceKey, this.Descriptors[i].ServiceKey))
                    {
                        if (ShouldCreateOpenGeneric(this.Descriptors[i].ServiceType, cacheKey.ServiceType))
                        {
                            // For AnyKey, we want to cache based on descriptor identity, not AnyKey that cacheKey has.
                            ServiceIdentifier registrationKey = isAnyKeyLookup ? ServiceIdentifier.FromDescriptor(this.Descriptors[i]) : cacheKey;
                            slot = GetSlot(registrationKey);
                            if (CreateOpenGeneric(this.Descriptors[i], registrationKey, callSiteChain, slot, throwOnConstraintViolation: false) is { } callSite)
                            {
                                AddCallSite(callSite, i);
                                UpdateSlot(registrationKey);
                            }
                        }
                    }
                }

                callSitesByIndex.Sort((a, b) => a.Key.CompareTo(b.Key));
                callSites = new ServiceCallSite[callSitesByIndex.Count];
                for (int i = 0; i < callSites.Length; ++i)
                {
                    callSites[i] = callSitesByIndex[i].Value;
                }

                void AddCallSite(ServiceCallSite callSite, int index)
                {
                    cacheLocation = GetCommonCacheLocation(cacheLocation, callSite.Cache.Location);
                    callSitesByIndex.Add(new(index, callSite));
                }

                int GetSlot(ServiceIdentifier key)
                {
                    if (!isAnyKeyLookup)
                    {
                        return slot;
                    }

                    // Each unique key (including its service type) maintains its own slot counter for ordering and identity.
                    if (keyedSlotAssignment is null)
                    {
                        keyedSlotAssignment = new(capacity: this.Descriptors.Length)
                        {
                            { key, 0 },
                        };

                        return 0;
                    }

                    if (keyedSlotAssignment.TryGetValue(key, out int existingSlot))
                    {
                        return existingSlot;
                    }

                    keyedSlotAssignment.Add(key, 0);
                    return 0;
                }

                void UpdateSlot(ServiceIdentifier key)
                {
                    if (!isAnyKeyLookup)
                    {
                        slot++;
                    }
                    else
                    {
                        Debug.Assert(keyedSlotAssignment is not null);
                        keyedSlotAssignment[key] = slot + 1;
                    }
                }
            }

            ResultCache resultCache = cacheLocation is CallSiteResultCacheLocation.Scope or CallSiteResultCacheLocation.Root
                ? new(cacheLocation, callSiteKey)
                : new(CallSiteResultCacheLocation.None, callSiteKey);

            return callSiteCache[callSiteKey] = new IEnumerableCallSite(resultCache, itemType, callSites, serviceIdentifier.ServiceKey);
        }
        finally
        {
            callSiteChain.Remove(serviceIdentifier);
        }

        static bool KeysMatch(object? lookupKey, object? descriptorKey)
        {
            if (lookupKey == null && descriptorKey == null)
            {
                // Both are non keyed services
                return true;
            }

            if (lookupKey != null && descriptorKey != null)
            {
                // Both are keyed services

                // We don't want to return AnyKey registration, so ignore it
                if (descriptorKey.Equals(KeyedService.AnyKey))
                {
                    return false;
                }

                // Check if both keys are equal, or if the lookup key
                // should match all keys (except AnyKey)
                return lookupKey.Equals(descriptorKey)
                    || lookupKey.Equals(KeyedService.AnyKey);
            }

            // One is a keyed service, one is not
            return false;
        }
    }

    private ServiceCallSite? TryCreateExact(ServiceDescriptor descriptor, ServiceIdentifier serviceIdentifier, CallSiteChain callSiteChain, int slot)
    {
        if (ShouldCreateExact(descriptor.ServiceType, serviceIdentifier.ServiceType))
        {
            return CreateExact(descriptor, serviceIdentifier, callSiteChain, slot);
        }

        return null;
    }

    private ServiceCallSite CreateExact(ServiceDescriptor descriptor, ServiceIdentifier serviceIdentifier, CallSiteChain callSiteChain, int slot)
    {
        ServiceCacheKey callSiteKey = new(serviceIdentifier, slot);
        if (callSiteCache.TryGetValue(callSiteKey, out ServiceCallSite? serviceCallSite))
        {
            return serviceCallSite;
        }

        ServiceCallSite callSite;
        ResultCache lifetime = new(descriptor.Lifetime, serviceIdentifier, slot);
        if (descriptor.HasImplementationInstance())
        {
            callSite = new ConstantCallSite(descriptor.ServiceType, descriptor.GetImplementationInstance(), descriptor.ServiceKey);
        }
        else if (descriptor is { IsKeyedService: false, ImplementationFactory: not null })
        {
            callSite = new FactoryCallSite(lifetime, descriptor.ServiceType, descriptor.ImplementationFactory);
        }
        else if (descriptor is { IsKeyedService: true, KeyedImplementationFactory: not null })
        {
            callSite = new FactoryCallSite(lifetime, descriptor.ServiceType, serviceIdentifier.ServiceKey!, descriptor.KeyedImplementationFactory);
        }
        else if (descriptor.HasImplementationType())
        {
            callSite = CreateConstructorCallSite(lifetime, serviceIdentifier, descriptor.GetImplementationType()!, callSiteChain);
        }
        else
        {
            throw new InvalidOperationException("Invalid service descriptor");
        }

        Debug.Assert(callSite != null);
        return callSiteCache[callSiteKey] = callSite;
    }

    private ServiceCallSite? TryCreateOpenGeneric(ServiceDescriptor descriptor, ServiceIdentifier serviceIdentifier, CallSiteChain callSiteChain, int slot, bool throwOnConstraintViolation)
    {
        if (ShouldCreateOpenGeneric(descriptor.ServiceType, serviceIdentifier.ServiceType))
        {
            return CreateOpenGeneric(descriptor, serviceIdentifier, callSiteChain, slot, throwOnConstraintViolation);
        }

        return null;
    }

    [UnconditionalSuppressMessage("ReflectionAnalysis", "IL2055:MakeGenericType", Justification = "MakeGenericType here is used to create a closed generic implementation type given the closed service type. Trimming annotations on the generic types are verified when 'Microsoft.Extensions.DependencyInjection.VerifyOpenGenericServiceTrimmability' is set, which is set by default when PublishTrimmed=true. That check informs developers when these generic types don't have compatible trimming annotations.")]
    [UnconditionalSuppressMessage("AotAnalysis", "IL3050:RequiresDynamicCode", Justification = "When ServiceProvider.VerifyAotCompatibility is true, which it is by default when PublishAot=true, this method ensures the generic types being created aren't using ValueTypes.")]
    private ServiceCallSite? CreateOpenGeneric(ServiceDescriptor descriptor, ServiceIdentifier serviceIdentifier, CallSiteChain callSiteChain, int slot, bool throwOnConstraintViolation)
    {
        ServiceCacheKey callSiteKey = new ServiceCacheKey(serviceIdentifier, slot);
        if (callSiteCache.TryGetValue(callSiteKey, out ServiceCallSite? serviceCallSite))
        {
            return serviceCallSite;
        }

        Type? implementationType = descriptor.GetImplementationType();
        Debug.Assert(implementationType != null, "descriptor.ImplementationType != null");
        ResultCache lifetime = new(descriptor.Lifetime, serviceIdentifier, slot);
        Type closedType;
        try
        {
            Type[] genericTypeArguments = serviceIdentifier.ServiceType.GenericTypeArguments;
            if (ServiceProvider.VerifyAotCompatibility)
            {
                VerifyOpenGenericAotCompatibility(serviceIdentifier.ServiceType, genericTypeArguments);
            }

            closedType = implementationType.MakeGenericType(genericTypeArguments);
        }
        catch (ArgumentException)
        {
            if (throwOnConstraintViolation)
            {
                throw;
            }

            return null;
        }

        ConstructorCallSite site = CreateConstructorCallSite(lifetime, serviceIdentifier, closedType, callSiteChain);
        Debug.Assert(site != null);
        return callSiteCache[callSiteKey] = site;
    }

    private ConstructorCallSite CreateConstructorCallSite(
        ResultCache lifetime,
        ServiceIdentifier serviceIdentifier,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]
        Type implementationType,
        CallSiteChain callSiteChain)
    {
        try
        {
            callSiteChain.Add(serviceIdentifier, implementationType);
            ConstructorInfo[] constructors = implementationType.GetConstructors();

            ServiceCallSite[]? parameterCallSites = null;

            if (constructors.Length == 0)
            {
                throw new InvalidOperationException($"A suitable constructor for type '{implementationType}' could not be located. Ensure the type is concrete and services are registered for all parameters of a public constructor.");
            }
            else if (constructors.Length == 1)
            {
                ConstructorInfo constructor = constructors[0];
                ParameterInfo[] parameters = constructor.GetParameters();
                if (parameters.Length == 0)
                {
                    return new(lifetime, serviceIdentifier.ServiceType, constructor, serviceIdentifier.ServiceKey);
                }

                parameterCallSites = CreateArgumentCallSites(
                    serviceIdentifier,
                    implementationType,
                    callSiteChain,
                    parameters,
                    throwIfCallSiteNotFound: true)!;

                return new(lifetime, serviceIdentifier.ServiceType, constructor, parameterCallSites, serviceIdentifier.ServiceKey);
            }

            Array.Sort(constructors, static (a, b) => b.GetParameters().Length.CompareTo(a.GetParameters().Length));

            ConstructorInfo? bestConstructor = null;
            HashSet<Type>? bestConstructorParameterTypes = null;
            foreach (ConstructorInfo constructor in constructors)
            {
                ParameterInfo[] parameters = constructor.GetParameters();

                ServiceCallSite[]? currentParameterCallSites = CreateArgumentCallSites(
                    serviceIdentifier,
                    implementationType,
                    callSiteChain,
                    parameters,
                    throwIfCallSiteNotFound: false);

                if (currentParameterCallSites != null)
                {
                    if (bestConstructor == null)
                    {
                        bestConstructor = constructor;
                        parameterCallSites = currentParameterCallSites;
                    }
                    else
                    {
                        // Since we're visiting constructors in decreasing order of number of parameters,
                        // we'll only see ambiguities or supersets once we've seen a 'bestConstructor'.
                        if (bestConstructorParameterTypes == null)
                        {
                            bestConstructorParameterTypes = [];
                            foreach (ParameterInfo p in bestConstructor.GetParameters())
                            {
                                bestConstructorParameterTypes.Add(p.ParameterType);
                            }
                        }

                        foreach (ParameterInfo p in parameters)
                        {
                            if (!bestConstructorParameterTypes.Contains(p.ParameterType))
                            {
                                // Ambiguous match exception
                                throw new InvalidOperationException(string.Join(Environment.NewLine, $"Unable to activate type '{implementationType}'. The following constructors are ambiguous:", bestConstructor, constructor));
                            }
                        }
                    }
                }
            }

            if (bestConstructor == null)
            {
                throw new InvalidOperationException($"No constructor for type '{implementationType}' can be instantiated using services from the service container and default values.");
            }
            else
            {
                Debug.Assert(parameterCallSites != null);
                return new(lifetime, serviceIdentifier.ServiceType, bestConstructor, parameterCallSites, serviceIdentifier.ServiceKey);
            }
        }
        finally
        {
            callSiteChain.Remove(serviceIdentifier);
        }
    }

    /// <returns>Not <b>null</b> if <b>throwIfCallSiteNotFound</b> is true</returns>
    private ServiceCallSite[]? CreateArgumentCallSites(
        ServiceIdentifier serviceIdentifier,
        Type implementationType,
        CallSiteChain callSiteChain,
        ParameterInfo[] parameters,
        bool throwIfCallSiteNotFound)
    {
        ServiceCallSite[] parameterCallSites = new ServiceCallSite[parameters.Length];

        for (int index = 0; index < parameters.Length; index++)
        {
            ServiceCallSite? callSite = null;
            bool isKeyedParameter = false;
            Type parameterType = parameters[index].ParameterType;
            foreach (object attribute in parameters[index].GetCustomAttributes(true))
            {
                if (serviceIdentifier.ServiceKey != null && attribute is ServiceKeyAttribute)
                {
                    // Even though the parameter may be strongly typed, support 'object' if AnyKey is used.
                    if (serviceIdentifier.ServiceKey == KeyedService.AnyKey)
                    {
                        parameterType = typeof(object);
                    }
                    else if (parameterType != serviceIdentifier.ServiceKey.GetType()
                             && parameterType != typeof(object))
                    {
                        throw new InvalidOperationException("The type of the key used for lookup doesn't match the type in the constructor parameter with the ServiceKey attribute.");
                    }

                    callSite = new ConstantCallSite(parameterType, serviceIdentifier.ServiceKey);
                    break;
                }

                if (attribute is Microsoft.Extensions.DependencyInjection.FromKeyedServicesAttribute keyed)
                {
                    ServiceIdentifier parameterSvcId = new(keyed.Key, parameterType);
                    callSite = GetCallSite(parameterSvcId, callSiteChain);
                    isKeyedParameter = true;
                    break;
                }
            }

            if (!isKeyedParameter)
            {
                callSite ??= GetCallSite(ServiceIdentifier.FromServiceType(parameterType), callSiteChain);
            }

            if (callSite == null && ParameterDefaultValue.TryGetDefaultValue(parameters[index], out object? defaultValue))
            {
                callSite = new ConstantCallSite(parameterType, defaultValue);
            }

            if (callSite == null)
            {
                if (throwIfCallSiteNotFound)
                {
                    throw new InvalidOperationException($"Unable to resolve service for type '{parameterType}' while attempting to activate '{implementationType}'.");
                }

                return null;
            }

            parameterCallSites[index] = callSite;
        }

        return parameterCallSites;
    }

    private struct ServiceDescriptorCacheItem
    {
        [DisallowNull]
        private ServiceDescriptor? item;

        [DisallowNull]
        private List<ServiceDescriptor>? items;

        public ServiceDescriptor Last
        {
            get
            {
                if (items is { Count: > 0 })
                {
                    return items[^1];
                }

                Debug.Assert(item != null);
                return item;
            }
        }

        public int Count
        {
            get
            {
                if (item == null)
                {
                    Debug.Assert(items == null);
                    return 0;
                }

                return 1 + (items?.Count ?? 0);
            }
        }

        public ServiceDescriptor this[int index]
        {
            get
            {
                if (index >= Count)
                {
                    throw new ArgumentOutOfRangeException(nameof(index));
                }

                if (index == 0)
                {
                    return item!;
                }

                return items![index - 1];
            }
        }

        public int GetSlot(ServiceDescriptor descriptor)
        {
            if (descriptor == item)
            {
                return Count - 1;
            }

            if (items != null)
            {
                int index = items.IndexOf(descriptor);
                if (index != -1)
                {
                    return items.Count - (index + 1);
                }
            }

            throw new InvalidOperationException("Requested service descriptor doesn't exist.");
        }

        public ServiceDescriptorCacheItem Add(ServiceDescriptor descriptor)
        {
            ServiceDescriptorCacheItem newCacheItem = default;
            if (item == null)
            {
                Debug.Assert(items == null);
                newCacheItem.item = descriptor;
            }
            else
            {
                newCacheItem.item = item;
                newCacheItem.items = items ?? [];
                newCacheItem.items.Add(descriptor);
            }

            return newCacheItem;
        }
    }
}