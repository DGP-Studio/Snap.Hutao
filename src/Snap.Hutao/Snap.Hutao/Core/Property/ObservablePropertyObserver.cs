// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;

namespace Snap.Hutao.Core.Property;

internal sealed partial class ObservablePropertyObserver<TSource, T> : ObservableObject, IReadOnlyObservableProperty<T>
{
    private readonly IObservableProperty<TSource> source;
    private readonly Func<TSource, T> converter;
    private T field;

    public ObservablePropertyObserver(IObservableProperty<TSource> source, Func<TSource, T> converter)
    {
        this.source = source;
        this.converter = converter;

        field = converter(source.Value);
        this.source.WeakPropertyChanged(this, OnWeakSourceValueChanged);
    }

    public T Value
    {
        get => @field;
        private set => SetProperty(ref @field, value);
    }

    private static void OnWeakSourceValueChanged(ObservablePropertyObserver<TSource, T> self, object? sender, PropertyChangedEventArgs e)
    {
        self.Value = self.converter(self.source.Value);
    }
}