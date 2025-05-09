// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Diagnostics;

namespace Snap.Hutao.Core.DependencyInjection.Implementation.ServiceLookup;

internal sealed class IEnumerableCallSite : ServiceCallSite
{
    public IEnumerableCallSite(ResultCache cache, Type itemType, ServiceCallSite[] serviceCallSites, object? serviceKey = null)
        : base(cache, serviceKey)
    {
        Debug.Assert(!ServiceProvider.VerifyAotCompatibility || !itemType.IsValueType, "If VerifyAotCompatibility=true, an IEnumerableCallSite should not be created with a ValueType.");

        ItemType = itemType;
        ServiceCallSites = serviceCallSites;
    }

    [UnconditionalSuppressMessage("AotAnalysis", "IL3050:RequiresDynamicCode", Justification = "When ServiceProvider.VerifyAotCompatibility is true, which it is by default when PublishAot=true, CallSiteFactory ensures ItemType is not a ValueType.")]
    public override Type ServiceType { get => typeof(IEnumerable<>).MakeGenericType(ItemType); }

    [UnconditionalSuppressMessage("AotAnalysis", "IL3050:RequiresDynamicCode", Justification = "When ServiceProvider.VerifyAotCompatibility is true, which it is by default when PublishAot=true, CallSiteFactory ensures ItemType is not a ValueType.")]
    public override Type ImplementationType { get => ItemType.MakeArrayType(); }

    public override CallSiteKind Kind { get; } = CallSiteKind.IEnumerable;

    internal Type ItemType { get; }

    internal ServiceCallSite[] ServiceCallSites { get; }
}