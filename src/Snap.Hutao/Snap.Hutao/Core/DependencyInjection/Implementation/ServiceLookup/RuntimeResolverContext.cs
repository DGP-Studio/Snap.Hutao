// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Core.DependencyInjection.Implementation.ServiceLookup;

internal struct RuntimeResolverContext
{
    public ServiceProviderEngineScope Scope { get; set; }

    public RuntimeResolverLock AcquiredLocks { get; set; }
}