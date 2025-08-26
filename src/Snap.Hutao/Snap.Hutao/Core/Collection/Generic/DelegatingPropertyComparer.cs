// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using JetBrains.Annotations;

namespace Snap.Hutao.Core.Collection.Generic;

internal abstract class DelegatingPropertyComparer<T, TProperty> : IComparer<T>
    where T : class
{
    private readonly IComparer<TProperty> delegatedComparer;
    private readonly Func<T, TProperty> delegation;

    protected DelegatingPropertyComparer([RequireStaticDelegate] Func<T, TProperty> delegation, IComparer<TProperty> delegatedComparer)
    {
        this.delegation = delegation;
        this.delegatedComparer = delegatedComparer;
    }

    public int Compare(T? x, T? y)
    {
        return (x, y) switch
        {
            (null, not null) => -1,
            (not null, null) => 1,
            (null, null) => 0,
            (not null, not null) => delegatedComparer.Compare(delegation(x), delegation(y)),
        };
    }
}