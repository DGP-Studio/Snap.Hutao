// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.DependencyInjection.Implementation.ServiceLookup.Expressions;

namespace Snap.Hutao.Core.DependencyInjection.Implementation.ServiceLookup;

internal abstract class CompiledServiceProviderEngine : ServiceProviderEngine
{
    [RequiresDynamicCode("Creates DynamicMethods")]
    protected CompiledServiceProviderEngine(ServiceProvider provider)
    {
        ResolverBuilder = new(provider);
    }

#if IL_EMIT
        public ILEmitResolverBuilder ResolverBuilder { get; }
#else
    public ExpressionResolverBuilder ResolverBuilder { get; }
#endif

    public override Func<ServiceProviderEngineScope, object?> RealizeService(ServiceCallSite callSite)
    {
        return ResolverBuilder.Build(callSite);
    }
}