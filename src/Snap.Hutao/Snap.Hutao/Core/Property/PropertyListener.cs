// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;

namespace Snap.Hutao.Core.Property;

internal sealed partial class PropertyListener<TSource, T> : ObservableObject, IReadOnlyObservableProperty<T>
    where T : struct
{
    private readonly IObservableProperty<TSource> source;
    private readonly Func<TSource, T> converter;
    private T? field;

    public PropertyListener(IObservableProperty<TSource> source, Func<TSource, T> converter)
    {
        this.source = source;
        this.converter = converter;

        this.source.WeakPropertyChanged(this, OnWeakSourceValueChanged);
    }

    public T Value
    {
        get => @field ??= converter(source.Value);
        private set => SetProperty(ref @field, value);
    }

    private static void OnWeakSourceValueChanged(PropertyListener<TSource, T> self, object? sender, PropertyChangedEventArgs e)
    {
        self.Value = self.converter(self.source.Value);
    }
}