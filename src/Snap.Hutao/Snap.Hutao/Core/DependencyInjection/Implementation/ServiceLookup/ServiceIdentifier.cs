// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Core.DependencyInjection.Implementation.ServiceLookup;

internal readonly struct ServiceIdentifier : IEquatable<ServiceIdentifier>
{
    public ServiceIdentifier(Type serviceType)
    {
        ServiceType = serviceType;
    }

    public ServiceIdentifier(object? serviceKey, Type serviceType)
    {
        ServiceKey = serviceKey;
        ServiceType = serviceType;
    }

    public object? ServiceKey { get; }

    public Type ServiceType { get; }

    public static ServiceIdentifier FromDescriptor(ServiceDescriptor serviceDescriptor)
    {
        return new(serviceDescriptor.ServiceKey, serviceDescriptor.ServiceType);
    }

    public static ServiceIdentifier FromServiceType(Type type)
    {
        return new(null, type);
    }

    public bool Equals(ServiceIdentifier other)
    {
        if (ServiceKey == null && other.ServiceKey == null)
        {
            return ServiceType == other.ServiceType;
        }

        if (ServiceKey != null && other.ServiceKey != null)
        {
            return ServiceType == other.ServiceType && ServiceKey.Equals(other.ServiceKey);
        }

        return false;
    }

    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        return obj is ServiceIdentifier identifier && Equals(identifier);
    }

    public override int GetHashCode()
    {
        if (ServiceKey == null)
        {
            return ServiceType.GetHashCode();
        }

        unchecked
        {
            return (ServiceType.GetHashCode() * 397) ^ ServiceKey.GetHashCode();
        }
    }

    public ServiceIdentifier GetGenericTypeDefinition()
    {
        return new(ServiceKey, ServiceType.GetGenericTypeDefinition());
    }

    public override string? ToString()
    {
        if (ServiceKey == null)
        {
            return ServiceType.ToString();
        }

        return $"({ServiceKey}, {ServiceType})";
    }
}