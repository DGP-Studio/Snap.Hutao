// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Core.DependencyInjection.Implementation.ServiceLookup;

internal sealed class FactoryCallSite : ServiceCallSite
{
    public FactoryCallSite(ResultCache cache, Type serviceType, Func<IServiceProvider, object> factory)
        : base(cache, null)
    {
        Factory = factory;
        ServiceType = serviceType;
    }

    public FactoryCallSite(ResultCache cache, Type serviceType, object serviceKey, Func<IServiceProvider, object, object> factory)
        : base(cache, serviceKey)
    {
        Factory = sp => factory(sp, serviceKey);
        ServiceType = serviceType;
    }

    public Func<IServiceProvider, object> Factory { get; }

    public override Type ServiceType { get; }

    public override Type? ImplementationType { get => null; }

    public override CallSiteKind Kind { get; } = CallSiteKind.Factory;
}