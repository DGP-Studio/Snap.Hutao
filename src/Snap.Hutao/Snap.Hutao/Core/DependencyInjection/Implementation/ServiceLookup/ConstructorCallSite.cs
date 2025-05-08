// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.
using System.Reflection;

namespace Snap.Hutao.Core.DependencyInjection.Implementation.ServiceLookup;

internal sealed class ConstructorCallSite : ServiceCallSite
{
    public ConstructorCallSite(ResultCache cache, Type serviceType, ConstructorInfo constructorInfo, object? serviceKey)
        : this(cache, serviceType, constructorInfo, [], serviceKey)
    {
    }

    public ConstructorCallSite(ResultCache cache, Type serviceType, ConstructorInfo constructorInfo, ServiceCallSite[] parameterCallSites, object? serviceKey)
        : base(cache, serviceKey)
    {
        if (!serviceType.IsAssignableFrom(constructorInfo.DeclaringType))
        {
            throw new ArgumentException($"Implementation type '{constructorInfo.DeclaringType}' can't be converted to service type '{serviceType}'");
        }

        ServiceType = serviceType;
        ConstructorInfo = constructorInfo;
        ParameterCallSites = parameterCallSites;
    }

    public override Type ServiceType { get; }

    public override Type? ImplementationType { get => ConstructorInfo.DeclaringType; }

    public override CallSiteKind Kind { get; } = CallSiteKind.Constructor;

    internal ConstructorInfo ConstructorInfo { get; }

    internal ServiceCallSite[] ParameterCallSites { get; }
}