// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using Snap.Hutao.Core.ExceptionService;
using Snap.Hutao.Model;
using Snap.Hutao.Service;
using System.Collections.Immutable;

namespace Snap.Hutao.Core.Property;

internal sealed partial class ObservablePropertyNameValueWrapper<T> : ObservableObject, IObservableProperty<NameValue<T>?>
    where T : notnull
{
    private readonly IObservableProperty<T> target;
    private readonly ImmutableArray<NameValue<T>> array;
    private bool deferring;
    private bool isExternalSet = true;

    public ObservablePropertyNameValueWrapper(IObservableProperty<T> target, ImmutableArray<NameValue<T>> array)
    {
        this.target = target;
        this.array = array;

        target.WeakPropertyChanged(this, OnWeakTargetValueChanged);
    }

    public NameValue<T>? Value
    {
        get => field ??= Selection.Initialize(array, target.Value);
        set
        {
            if (Volatile.Read(ref deferring))
            {
                field = value;
                if (value is not null)
                {
                    target.Value = value.Value;
                }
            }
            else
            {
                if (SetProperty(ref field, value) && value is not null)
                {
                    Interlocked.Exchange(ref isExternalSet, false);
                    target.Value = value.Value;
                    Interlocked.Exchange(ref isExternalSet, true);
                }
            }
        }
    }

    public INotifyPropertyChangedDeferral GetDeferral()
    {
        if (Interlocked.Exchange(ref deferring, true))
        {
            throw HutaoException.InvalidOperation("Already deferring");
        }

        INotifyPropertyChangedDeferral targetDeferral = target.GetDeferral();
        return NotifyPropertyChangedDeferral.Create(this, targetDeferral, static (self, targetDeferral) =>
        {
            if (!Interlocked.Exchange(ref self.deferring, false))
            {
                throw HutaoException.InvalidOperation("Not deferring");
            }

            self.OnPropertyChanged(nameof(Value));
            using (targetDeferral)
            {
            }
        });
    }

    private static void OnWeakTargetValueChanged(ObservablePropertyNameValueWrapper<T> self, object? sender, PropertyChangedEventArgs e)
    {
        if (Volatile.Read(ref self.isExternalSet))
        {
            self.Value = Selection.Initialize(self.array, self.target.Value);
        }
    }
}