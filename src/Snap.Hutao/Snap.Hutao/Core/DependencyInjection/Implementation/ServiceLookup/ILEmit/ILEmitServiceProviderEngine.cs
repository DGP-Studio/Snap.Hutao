// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Core.DependencyInjection.Implementation.ServiceLookup.ILEmit;

internal sealed class ILEmitServiceProviderEngine : ServiceProviderEngine
{
    private readonly ILEmitResolverBuilder expressionResolverBuilder;

    [RequiresDynamicCode("Creates DynamicMethods")]
    public ILEmitServiceProviderEngine(ServiceProvider serviceProvider)
    {
        expressionResolverBuilder = new(serviceProvider);
    }

    public override Func<ServiceProviderEngineScope, object?> RealizeService(ServiceCallSite callSite)
    {
        return expressionResolverBuilder.Build(callSite);
    }
}