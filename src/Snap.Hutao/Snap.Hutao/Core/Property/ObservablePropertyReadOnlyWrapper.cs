// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Core.Property;

internal sealed partial class ObservablePropertyReadOnlyWrapper<T> : IReadOnlyObservableProperty<T>
{
    private readonly IObservableProperty<T> source;

    public ObservablePropertyReadOnlyWrapper(IObservableProperty<T> source)
    {
        this.source = source;
    }

    public event PropertyChangedEventHandler? PropertyChanged
    {
        add => source.PropertyChanged += value;
        remove => source.PropertyChanged -= value;
    }

    public T Value
    {
        get => source.Value;
    }
}