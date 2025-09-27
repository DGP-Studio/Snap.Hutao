// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Core.Property;

internal sealed partial class ObservablePropertyWithConditionalSetMethod<T, TState> : IObservableProperty<T>
{
    private readonly IObservableProperty<T> source;
    private readonly Func<T, TState, bool> condition;
    private readonly TState state;

    public ObservablePropertyWithConditionalSetMethod(IObservableProperty<T> source, Func<T, TState, bool> condition, TState state)
    {
        this.source = source;
        this.condition = condition;
        this.state = state;
    }

    public event PropertyChangedEventHandler? PropertyChanged
    {
        add => source.PropertyChanged += value;
        remove => source.PropertyChanged -= value;
    }

    public T Value
    {
        get => source.Value;
        set
        {
            if (condition(value, state))
            {
                source.Value = value;
            }
        }
    }

    public INotifyPropertyChangedDeferral GetDeferral()
    {
        return source.GetDeferral();
    }
}