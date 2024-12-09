// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Core.ComponentModel;

internal sealed class AsyncDisposableObservableBox<TNotifyPropertyChanged, T> : IAsyncDisposableObservableBox<T>
    where TNotifyPropertyChanged : INotifyPropertyChanged
    where T : class?, IDisposable?
{
    private readonly TNotifyPropertyChanged source;
    private readonly Func<TNotifyPropertyChanged, T> valueFactory;
    private readonly string propertyName;

    private volatile CancellationTokenSource? currentCts;
    private volatile TaskCompletionSource? currentTcs;
    private bool isDisposed;

    public AsyncDisposableObservableBox(T value, TNotifyPropertyChanged source, string propertyName, Func<TNotifyPropertyChanged, T> valueFactory)
    {
        Value = value;
        this.source = source;
        this.propertyName = propertyName;
        this.valueFactory = valueFactory;
        source.PropertyChanged += OnPropertyChanged;
    }

    public T Value { get; private set; }

    public AsyncLock SyncRoot { get; } = new();

    public async ValueTask DisposeAsync()
    {
        if (isDisposed)
        {
            return;
        }

        using (await SyncRoot.LockAsync().ConfigureAwait(false))
        {
            if (isDisposed)
            {
                return;
            }

            isDisposed = true;
            source.PropertyChanged -= OnPropertyChanged;
            if (currentCts is not null)
            {
                await currentCts.CancelAsync().ConfigureAwait(false);

                if (currentTcs is not null)
                {
                    await currentTcs.Task.ConfigureAwait(false);
                }

                currentCts.Dispose();
            }

            Value?.Dispose();
            Value = default!;
        }
    }

    private void OnPropertyChanged(object? sender, PropertyChangedEventArgs args)
    {
        _ = OnPropertyChangedAsync(args);
    }

    [SuppressMessage("", "SH003")]
    private async Task OnPropertyChangedAsync(PropertyChangedEventArgs args)
    {
        if (currentCts is not null)
        {
            await currentCts.CancelAsync().ConfigureAwait(false);

            if (currentTcs is not null)
            {
                await currentTcs.Task.ConfigureAwait(false);
            }

            // Must await currentTcs before dispose currentCts.
            currentCts.Dispose();
        }

        // Capture current tcs reference.
        TaskCompletionSource tcs = new();
        currentTcs = tcs;
        currentCts = new();

        try
        {
            // Capture current cts reference.
            CancellationToken token = currentCts.Token;
            using (await SyncRoot.LockAsync().ConfigureAwait(false))
            {
                if (token.IsCancellationRequested)
                {
                    return;
                }

                if (args.PropertyName != propertyName)
                {
                    return;
                }

                Value?.Dispose();
                Value = valueFactory(source);
            }
        }
        finally
        {
            tcs.TrySetResult();
        }
    }
}