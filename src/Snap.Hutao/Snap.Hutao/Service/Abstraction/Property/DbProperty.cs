// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using Snap.Hutao.Core;
using Snap.Hutao.Model.Entity.Database;
using System.Runtime.CompilerServices;

namespace Snap.Hutao.Service.Abstraction.Property;

internal abstract class DbProperty<T> : ObservableObject, IProperty<T>
{
    public abstract T Value { get; set; }

    public static implicit operator T(DbProperty<T> property)
    {
        return property.Value;
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    protected static string? GetValue(AppDbContext appDbContext, string key)
    {
        // This method is separated to avoid implicit capture of the key
        return appDbContext.Settings.SingleOrDefault(e => e.Key == key)?.Value;
    }
}