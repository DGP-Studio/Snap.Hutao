// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using Snap.Hutao.Model;
using Snap.Hutao.Service;
using System.Collections.Immutable;

namespace Snap.Hutao.Core.Property;

internal sealed partial class PropertyNameValueWrapper<T> : ObservableObject, IObservableProperty<NameValue<T>?>
    where T : notnull
{
    private readonly IProperty<T> target;
    private readonly ImmutableArray<NameValue<T>> array;

    public PropertyNameValueWrapper(IProperty<T> target, ImmutableArray<NameValue<T>> array)
    {
        this.target = target;
        this.array = array;
    }

    public NameValue<T>? Value
    {
        get => field ??= Selection.Initialize(array, target.Value);
        set
        {
            if (SetProperty(ref field, value) && value is not null)
            {
                target.Value = value.Value;
            }
        }
    }
}