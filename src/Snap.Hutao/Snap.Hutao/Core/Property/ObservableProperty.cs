// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using Snap.Hutao.Core.ExceptionService;

namespace Snap.Hutao.Core.Property;

internal sealed partial class ObservableProperty<T> : ObservableObject, IObservableProperty<T>
{
    private bool deferring;
    private T field;

    public ObservableProperty(T value)
    {
        field = value;
    }

    public T Value
    {
        get => @field;
        set
        {
            if (Volatile.Read(ref deferring))
            {
                @field = value;
            }
            else
            {
                SetProperty(ref @field, value);
            }
        }
    }

    public INotifyPropertyChangedDeferral GetDeferral()
    {
        if (Interlocked.Exchange(ref deferring, true))
        {
            throw HutaoException.InvalidOperation("Already deferring");
        }

        return NotifyPropertyChangedDeferral.Create(this, static self =>
        {
            if (!Interlocked.Exchange(ref self.deferring, false))
            {
                throw HutaoException.InvalidOperation("Not deferring");
            }

            self.OnPropertyChanged(nameof(Value));
        });
    }
}