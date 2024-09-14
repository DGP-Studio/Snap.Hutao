// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Runtime.CompilerServices;

namespace Snap.Hutao.Core.ComponentModel;

internal sealed partial class NotifyPropertyChangedBox<TNotifyPropertyChanged, T> : StrongBox<T>, IDisposable
    where TNotifyPropertyChanged : INotifyPropertyChanged
{
    private readonly TNotifyPropertyChanged notifyPropertyChanged;
    private readonly string propertyName;
    private readonly Func<TNotifyPropertyChanged, T> valueAccessor;

    public NotifyPropertyChangedBox(T value, TNotifyPropertyChanged notifyPropertyChanged, string propertyName, Func<TNotifyPropertyChanged, T> valueAccessor)
        : base(value)
    {
        this.notifyPropertyChanged = notifyPropertyChanged;
        this.propertyName = propertyName;
        this.valueAccessor = valueAccessor;
        notifyPropertyChanged.PropertyChanged += OnPropertyChanged;
    }

    public void Dispose()
    {
        notifyPropertyChanged.PropertyChanged -= OnPropertyChanged;

        if (Value is IDisposable disposable)
        {
            disposable.Dispose();
        }
    }

    private void OnPropertyChanged(object? sender, PropertyChangedEventArgs args)
    {
        if (args.PropertyName != propertyName)
        {
            return;
        }

        if (Value is IDisposable disposable)
        {
            disposable.Dispose();
        }

        Value = valueAccessor(notifyPropertyChanged);
    }
}