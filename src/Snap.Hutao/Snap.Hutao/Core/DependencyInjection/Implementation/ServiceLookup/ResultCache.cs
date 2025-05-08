// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Core.DependencyInjection.Implementation.ServiceLookup;

internal struct ResultCache
{
    public ResultCache(ServiceLifetime lifetime, ServiceIdentifier serviceIdentifier, int slot)
    {
        Location = lifetime switch
        {
            ServiceLifetime.Singleton => CallSiteResultCacheLocation.Root,
            ServiceLifetime.Scoped => CallSiteResultCacheLocation.Scope,
            ServiceLifetime.Transient => CallSiteResultCacheLocation.Dispose,
            _ => CallSiteResultCacheLocation.None,
        };
        Key = new(serviceIdentifier, slot);
    }

    internal ResultCache(CallSiteResultCacheLocation lifetime, ServiceCacheKey cacheKey)
    {
        Location = lifetime;
        Key = cacheKey;
    }

    public CallSiteResultCacheLocation Location { get; set; }

    public ServiceCacheKey Key { get; set; }

    public static ResultCache None(Type serviceType)
    {
        ServiceCacheKey cacheKey = new(ServiceIdentifier.FromServiceType(serviceType), 0);
        return new(CallSiteResultCacheLocation.None, cacheKey);
    }
}