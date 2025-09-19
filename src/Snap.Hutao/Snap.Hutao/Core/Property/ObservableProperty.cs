// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;

namespace Snap.Hutao.Core.Property;

internal sealed partial class ObservableProperty<T> : ObservableObject, IObservableProperty<T>
{
    private T field;

    public ObservableProperty(T value)
    {
        field = value;
    }

    public T Value
    {
        get => @field;
        set => SetProperty(ref @field, value);
    }
}