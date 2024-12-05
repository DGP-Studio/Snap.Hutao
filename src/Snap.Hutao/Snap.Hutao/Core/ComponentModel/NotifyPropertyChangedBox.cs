// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Runtime.CompilerServices;

namespace Snap.Hutao.Core.ComponentModel;

internal sealed partial class NotifyPropertyChangedBox<TNotifyPropertyChanged, T> : StrongBox<T>, IDisposable
    where TNotifyPropertyChanged : INotifyPropertyChanged
    where T : class, IDisposable
{
    private readonly TNotifyPropertyChanged source;
    private readonly Func<TNotifyPropertyChanged, T> valueFactory;
    private readonly string propertyName;

    public NotifyPropertyChangedBox(T value, TNotifyPropertyChanged source, string propertyName, Func<TNotifyPropertyChanged, T> valueFactory)
        : base(value)
    {
        this.source = source;
        this.propertyName = propertyName;
        this.valueFactory = valueFactory;
        source.PropertyChanged += OnPropertyChanged;
    }

    public void Dispose()
    {
        source.PropertyChanged -= OnPropertyChanged;
        Value?.Dispose();
    }

    private void OnPropertyChanged(object? sender, PropertyChangedEventArgs args)
    {
        if (args.PropertyName != propertyName)
        {
            return;
        }

        Value?.Dispose();
        Value = valueFactory(source);
    }
}