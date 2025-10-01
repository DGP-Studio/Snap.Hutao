// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Core.Property;

internal sealed partial class ObservablePropertyValueChangedCallbackWrapper<T> : IObservableProperty<T>
{
    private readonly IObservableProperty<T> source;
    private readonly Action<T> callback;

    public ObservablePropertyValueChangedCallbackWrapper(IObservableProperty<T> source, Action<T> callback)
    {
        this.source = source;
        this.callback = callback;

        this.source.WeakPropertyChanged(this, OnWeakSourceValueChanged);
    }

    public event PropertyChangedEventHandler? PropertyChanged
    {
        add => source.PropertyChanged += value;
        remove => source.PropertyChanged -= value;
    }

    public T Value
    {
        get => source.Value;
        set => source.Value = value;
    }

    public INotifyPropertyChangedDeferral GetDeferral()
    {
        return source.GetDeferral();
    }

    private static void OnWeakSourceValueChanged(ObservablePropertyValueChangedCallbackWrapper<T> self, object? sender, PropertyChangedEventArgs e)
    {
        self.callback(self.source.Value);
    }
}

internal sealed partial class ObservablePropertyValueChangedCallbackWrapper<T, TState> : IObservableProperty<T>
{
    private readonly IObservableProperty<T> source;
    private readonly Action<T, TState> callback;
    private readonly TState state;

    public ObservablePropertyValueChangedCallbackWrapper(IObservableProperty<T> source, Action<T, TState> callback, TState state)
    {
        this.source = source;
        this.callback = callback;
        this.state = state;

        this.source.WeakPropertyChanged(this, OnWeakSourceValueChanged);
    }

    public event PropertyChangedEventHandler? PropertyChanged
    {
        add => source.PropertyChanged += value;
        remove => source.PropertyChanged -= value;
    }

    public T Value
    {
        get => source.Value;
        set => source.Value = value;
    }

    public INotifyPropertyChangedDeferral GetDeferral()
    {
        return source.GetDeferral();
    }

    private static void OnWeakSourceValueChanged(ObservablePropertyValueChangedCallbackWrapper<T, TState> self, object? sender, PropertyChangedEventArgs e)
    {
        self.callback(self.source.Value, self.state);
    }
}