// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Core.DependencyInjection.Implementation.ServiceLookup;

internal sealed class RuntimeServiceProviderEngine : ServiceProviderEngine
{
    private RuntimeServiceProviderEngine()
    {
    }

    public static RuntimeServiceProviderEngine Instance { get; } = new();

    public override Func<ServiceProviderEngineScope, object?> RealizeService(ServiceCallSite callSite)
    {
        return scope => CallSiteRuntimeResolver.Instance.Resolve(callSite, scope);
    }
}