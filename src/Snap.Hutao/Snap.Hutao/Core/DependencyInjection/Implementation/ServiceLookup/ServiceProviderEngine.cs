// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Core.DependencyInjection.Implementation.ServiceLookup;

internal abstract class ServiceProviderEngine
{
    public abstract Func<ServiceProviderEngineScope, object?> RealizeService(ServiceCallSite callSite);
}