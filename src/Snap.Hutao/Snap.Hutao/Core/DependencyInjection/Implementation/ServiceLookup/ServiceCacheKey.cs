// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Core.DependencyInjection.Implementation.ServiceLookup;

internal readonly struct ServiceCacheKey : IEquatable<ServiceCacheKey>
{
    public ServiceCacheKey(object key, Type type, int slot)
    {
        ServiceIdentifier = new(key, type);
        Slot = slot;
    }

    public ServiceCacheKey(ServiceIdentifier type, int slot)
    {
        ServiceIdentifier = type;
        Slot = slot;
    }

    /// <summary>
    /// Type of service being cached
    /// </summary>
    public ServiceIdentifier ServiceIdentifier { get; }

    /// <summary>
    /// Reverse index of the service when resolved in <c>IEnumerable&lt;Type&gt;</c> where default instance gets slot 0.
    /// For example for service collection
    ///  IService Impl1
    ///  IService Impl2
    ///  IService Impl3
    /// We would get the following cache keys:
    ///  Impl1 2
    ///  Impl2 1
    ///  Impl3 0
    /// </summary>
    public int Slot { get; }

    /// <summary>Indicates whether the current instance is equal to another instance of the same type.</summary>
    /// <param name="other">An instance to compare with this instance.</param>
    /// <returns>true if the current instance is equal to the other instance; otherwise, false.</returns>
    public bool Equals(ServiceCacheKey other)
    {
        return ServiceIdentifier.Equals(other.ServiceIdentifier) && Slot == other.Slot;
    }

    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        return obj is ServiceCacheKey other && Equals(other);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            return (ServiceIdentifier.GetHashCode() * 397) ^ Slot;
        }
    }
}