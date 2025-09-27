// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Core.Property;

internal sealed partial class ObservablePropertyWithConditionalSetMethod<T> : IObservableProperty<T>
{
    private readonly IObservableProperty<T> source;
    private readonly IProperty<bool> condition;

    public ObservablePropertyWithConditionalSetMethod(IObservableProperty<T> source, IProperty<bool> condition)
    {
        this.source = source;
        this.condition = condition;
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
            if (condition.Value)
            {
                source.Value = value;
            }
        }
    }
}