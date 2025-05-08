// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Core.DependencyInjection.Implementation.ServiceLookup;

internal static class ServiceDescriptorExtensions
{
    public static bool HasImplementationInstance(this ServiceDescriptor serviceDescriptor)
    {
        return GetImplementationInstance(serviceDescriptor) != null;
    }

    public static bool HasImplementationFactory(this ServiceDescriptor serviceDescriptor)
    {
        return GetImplementationFactory(serviceDescriptor) != null;
    }

    public static bool HasImplementationType(this ServiceDescriptor serviceDescriptor)
    {
        return GetImplementationType(serviceDescriptor) != null;
    }

    public static object? GetImplementationInstance(this ServiceDescriptor serviceDescriptor)
    {
        return serviceDescriptor.IsKeyedService
            ? serviceDescriptor.KeyedImplementationInstance
            : serviceDescriptor.ImplementationInstance;
    }

    public static object? GetImplementationFactory(this ServiceDescriptor serviceDescriptor)
    {
        return serviceDescriptor.IsKeyedService
            ? serviceDescriptor.KeyedImplementationFactory
            : serviceDescriptor.ImplementationFactory;
    }

    [return: DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]
    public static Type? GetImplementationType(this ServiceDescriptor serviceDescriptor)
    {
        return serviceDescriptor.IsKeyedService
            ? serviceDescriptor.KeyedImplementationType
            : serviceDescriptor.ImplementationType;
    }

    public static bool TryGetImplementationType(this ServiceDescriptor serviceDescriptor, out Type? type)
    {
        type = GetImplementationType(serviceDescriptor);
        return type != null;
    }
}