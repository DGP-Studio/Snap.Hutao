// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;

namespace Snap.Hutao.Core.Property;

internal sealed partial class PropertyLinker<TSource, TTarget> : ObservableObject, IObservableProperty<TSource>
{
    private readonly IObservableProperty<TSource> source;
    private readonly IProperty<TTarget> target;
    private readonly Action<TSource, IProperty<TTarget>> callback;

    public PropertyLinker(IObservableProperty<TSource> source, IProperty<TTarget> target, Action<TSource, IProperty<TTarget>> callback)
    {
        this.source = source;
        this.target = target;
        this.callback = callback;

        this.source.WeakPropertyChanged(this, OnWeakSourceValueChanged);
    }

    public TSource Value
    {
        get => source.Value;
        set => SetProperty(source.Value, value, source, static (source, v) => source.Value = v);
    }

    private static void OnWeakSourceValueChanged(PropertyLinker<TSource, TTarget> self, object? sender, PropertyChangedEventArgs e)
    {
        self.callback(self.source.Value, self.target);
    }
}