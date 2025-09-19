// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using Snap.Hutao.Service;
using System.Collections.Immutable;

namespace Snap.Hutao.Core.Property;

internal sealed partial class PropertyNullableSelectionWrapper<T, TSource> : ObservableObject, IObservableProperty<T?>
    where T : class
    where TSource : notnull
{
    private readonly IProperty<TSource> source;
    private readonly Func<T?, TSource> valueSelector;
    private T? field;

    public PropertyNullableSelectionWrapper(IProperty<TSource> source, ImmutableArray<T> array, Func<T?, TSource> valueSelector, IEqualityComparer<TSource> equalityComparer)
    {
        this.source = source;
        this.valueSelector = valueSelector;

        field = Selection.Initialize(array, source.Value, valueSelector, equalityComparer);
    }

    public T? Value
    {
        get => @field;
        set
        {
            if (SetProperty(ref @field, value))
            {
                source.Value = valueSelector(value);
            }
        }
    }
}