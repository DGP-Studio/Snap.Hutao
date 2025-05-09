// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Core.DependencyInjection.Implementation.ServiceLookup;

internal sealed class ServiceProviderCallSite : ServiceCallSite
{
    public ServiceProviderCallSite()
        : base(ResultCache.None(typeof(IServiceProvider)), null)
    {
    }

    public override Type ServiceType { get; } = typeof(IServiceProvider);

    public override Type ImplementationType { get; } = typeof(ServiceProvider);

    public override CallSiteKind Kind { get; } = CallSiteKind.ServiceProvider;
}