// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Core.DependencyInjection.Implementation.ServiceLookup;

internal abstract class ServiceCallSite
{
    protected ServiceCallSite(ResultCache cache, object? key)
    {
        Cache = cache;
        Key = key;
    }

    public abstract Type ServiceType { get; }

    public abstract Type? ImplementationType { get; }

    public abstract CallSiteKind Kind { get; }

    public ResultCache Cache { get; }

    public object? Value { get; set; }

    public object? Key { get; }

    public bool CaptureDisposable
    {
        get
        {
            return ImplementationType == null
                || typeof(IDisposable).IsAssignableFrom(ImplementationType)
                || typeof(IAsyncDisposable).IsAssignableFrom(ImplementationType);
        }
    }
}