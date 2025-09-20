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
    private readonly IObservableProperty<T> target;
    private readonly ImmutableArray<NameValue<T>> array;
    private bool outside = true;

    public PropertyNameValueWrapper(IObservableProperty<T> target, ImmutableArray<NameValue<T>> array)
    {
        this.target = target;
        this.array = array;

        target.WeakPropertyChanged(this, OnWeakTargetValueChanged);
    }

    public NameValue<T>? Value
    {
        get => field ??= Selection.Initialize(array, target.Value);
        set
        {
            if (SetProperty(ref field, value) && value is not null)
            {
                Interlocked.Exchange(ref outside, false);
                target.Value = value.Value;
                Interlocked.Exchange(ref outside, true);
            }
        }
    }

    private static void OnWeakTargetValueChanged(PropertyNameValueWrapper<T> self, object? sender, PropertyChangedEventArgs e)
    {
        if (Volatile.Read(ref self.outside))
        {
            self.Value = Selection.Initialize(self.array, self.target.Value);
        }
    }
}