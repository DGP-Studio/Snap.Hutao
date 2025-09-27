// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using Snap.Hutao.Core.ExceptionService;
using Snap.Hutao.Core.Property;
using Snap.Hutao.Model.Entity.Database;
using System.Runtime.CompilerServices;

namespace Snap.Hutao.Service.Abstraction.Property;

internal abstract class DbProperty<T> : ObservableObject, IObservableProperty<T>
{
    private bool deferring;

    public abstract T Value { get; set; }

    protected ref bool Deferring
    {
        get => ref deferring;
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

    [MethodImpl(MethodImplOptions.NoInlining)]
    protected static string? GetValue(AppDbContext appDbContext, string key)
    {
        // This method is separated to avoid implicit capture of the key
        return appDbContext.Settings.SingleOrDefault(e => e.Key == key)?.Value;
    }

    protected abstract void SetValue(T value);
}