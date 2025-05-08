// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.
using System.Runtime.CompilerServices;

namespace Snap.Hutao.Core.DependencyInjection.Implementation.ServiceLookup;

internal static class ThrowHelper
{
    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.NoInlining)]
    internal static void ThrowObjectDisposedException(bool isRootScope)
    {
        throw new ServiceProviderDisposedException(isRootScope);
    }

    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.NoInlining)]
    internal static void ThrowInvalidOperationException_KeyedServiceAnyKeyUsedToResolveService()
    {
        throw new InvalidOperationException("KeyedService.AnyKey cannot be used to resolve a single service.");
    }

    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.NoInlining)]
    internal static void ThrowInvalidOperationException_NoServiceRegistered(Type serviceType)
    {
        throw new InvalidOperationException($"No service for type '{serviceType}' has been registered.");
    }

    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.NoInlining)]
    internal static void ThrowInvalidOperationException_NoKeyedServiceRegistered(Type serviceType, Type keyType)
    {
        throw new InvalidOperationException($"No keyed service for type '{serviceType}' using key type '{keyType}' has been registered.");
    }
}