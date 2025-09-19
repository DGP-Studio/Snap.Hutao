// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;

namespace Snap.Hutao.Core.Property;

internal sealed partial class PropertyValueChangedCallbackWrapper<T> : ObservableObject, IObservableProperty<T>
{
    private readonly IObservableProperty<T> source;
    private readonly Action<T> callback;

    public PropertyValueChangedCallbackWrapper(IObservableProperty<T> source, Action<T> callback)
    {
        this.source = source;
        this.callback = callback;

        this.source.WeakPropertyChanged(this, OnWeakSourceValueChanged);
    }

    public T Value
    {
        get => source.Value;
        set => SetProperty(source.Value, value, source, static (prop, v) => prop.Value = v);
    }

    private static void OnWeakSourceValueChanged(PropertyValueChangedCallbackWrapper<T> self, object? sender, PropertyChangedEventArgs e)
    {
        self.callback(self.source.Value);
    }
}

internal sealed partial class PropertyValueChangedCallbackWrapper<T, TState> : ObservableObject, IObservableProperty<T>
{
    private readonly IObservableProperty<T> source;
    private readonly Action<T, TState> callback;
    private readonly TState state;

    public PropertyValueChangedCallbackWrapper(IObservableProperty<T> source, Action<T, TState> callback, TState state)
    {
        this.source = source;
        this.callback = callback;
        this.state = state;

        this.source.WeakPropertyChanged(this, OnWeakSourceValueChanged);
    }

    public T Value
    {
        get => source.Value;
        set => SetProperty(source.Value, value, source, static (prop, v) => prop.Value = v);
    }

    private static void OnWeakSourceValueChanged(PropertyValueChangedCallbackWrapper<T, TState> self, object? sender, PropertyChangedEventArgs e)
    {
        self.callback(self.source.Value, self.state);
    }
}