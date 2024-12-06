// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Core.ComponentModel;

internal sealed partial class DisposableObservableBox<TNotifyPropertyChanged, T> : IDisposableObservableBox<T>
    where TNotifyPropertyChanged : INotifyPropertyChanged
    where T : class?, IDisposable?
{
    private readonly TNotifyPropertyChanged source;
    private readonly Func<TNotifyPropertyChanged, T> valueFactory;
    private readonly string propertyName;

    private readonly Lock syncRoot = new();
    private bool isDisposed;

    public DisposableObservableBox(T value, TNotifyPropertyChanged source, string propertyName, Func<TNotifyPropertyChanged, T> valueFactory)
    {
        Value = value;
        this.source = source;
        this.propertyName = propertyName;
        this.valueFactory = valueFactory;
        source.PropertyChanged += OnPropertyChanged;
    }

    public T Value { get; private set; }

    public void Dispose()
    {
        if (isDisposed)
        {
            return;
        }

        lock (syncRoot)
        {
            if (isDisposed)
            {
                return;
            }

            isDisposed = true;
            source.PropertyChanged -= OnPropertyChanged;
            Value?.Dispose();
            Value = default!;
        }
    }

    private void OnPropertyChanged(object? sender, PropertyChangedEventArgs args)
    {
        lock (syncRoot)
        {
            if (args.PropertyName != propertyName)
            {
                return;
            }

            Value?.Dispose();
            Value = valueFactory(source);
        }
    }
}