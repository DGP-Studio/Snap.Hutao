// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using Snap.Hutao.Core.ExceptionService;
using System.Runtime.CompilerServices;

namespace Snap.Hutao.ViewModel.Abstraction;

/// <summary>
/// 视图模型抽象类
/// </summary>
[HighQuality]
internal abstract partial class ViewModel : ObservableObject, IViewModel
{
    private bool isInitialized;

    public bool IsInitialized { get => isInitialized; set => SetProperty(ref isInitialized, value); }

    public CancellationToken CancellationToken { get; set; }

    public SemaphoreSlim DisposeLock { get; set; } = new(1);

    public bool IsViewDisposed { get; set; }

    protected TaskCompletionSource<bool> Initialization { get; } = new();

    [Command("OpenUICommand")]
    protected virtual async Task OpenUIAsync()
    {
        // Set value on UI thread
        IsInitialized = await InitializeUIAsync().ConfigureAwait(true);
        Initialization.TrySetResult(IsInitialized);
    }

    protected virtual ValueTask<bool> InitializeUIAsync()
    {
        return ValueTask.FromResult(true);
    }

    protected async ValueTask<IDisposable> EnterCriticalExecutionAsync()
    {
        ThrowIfViewDisposed();
        IDisposable disposable = await DisposeLock.EnterAsync(CancellationToken).ConfigureAwait(false);
        ThrowIfViewDisposed();
        return disposable;
    }

    protected bool SetProperty<T>(ref T storage, T value, Action<T> changedCallback, [CallerMemberName] string? propertyName = null)
    {
        if (SetProperty(ref storage, value, propertyName))
        {
            changedCallback(value);
            return true;
        }

        return false;
    }

    protected bool SetProperty<T>(ref T storage, T value, Func<T, ValueTask> changedAsyncCallback, [CallerMemberName] string? propertyName = null)
    {
        if (SetProperty(ref storage, value, propertyName))
        {
            changedAsyncCallback(value).SafeForget();
            return true;
        }

        return false;
    }

    private void ThrowIfViewDisposed()
    {
        if (IsViewDisposed)
        {
            ThrowHelper.OperationCanceled(SH.ViewModelViewDisposedOperationCancel);
        }
    }
}