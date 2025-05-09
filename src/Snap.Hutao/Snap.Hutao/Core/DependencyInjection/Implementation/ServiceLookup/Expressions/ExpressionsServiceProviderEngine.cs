// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Core.DependencyInjection.Implementation.ServiceLookup.Expressions;

internal sealed class ExpressionsServiceProviderEngine : ServiceProviderEngine
{
    private readonly ExpressionResolverBuilder expressionResolverBuilder;

    public ExpressionsServiceProviderEngine(ServiceProvider serviceProvider)
    {
        expressionResolverBuilder = new(serviceProvider);
    }

    public override Func<ServiceProviderEngineScope, object> RealizeService(ServiceCallSite callSite)
    {
        return expressionResolverBuilder.Build(callSite);
    }
}