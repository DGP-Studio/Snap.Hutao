// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using Snap.Hutao.Core.ExceptionService;
using Snap.Hutao.UI.Xaml;
using System.Runtime.CompilerServices;

namespace Snap.Hutao.ViewModel.Abstraction;

[SuppressMessage("", "SA1124")]
internal abstract partial class ViewModel : ObservableObject, IViewModel
{
    public bool IsInitialized { get; set => SetProperty(ref field, value); }

    public CancellationToken CancellationToken { get; set; }

    public SemaphoreSlim DisposeLock { get; set; } = new(1);

    public IDeferContentLoader? DeferContentLoader { get; set; }

    public bool IsViewDisposed { get; set; }

    protected TaskCompletionSource<bool> Initialization { get; set; } = new();

    public void Resurrect()
    {
        IsViewDisposed = false;
        Initialization = new();
    }

    public void Uninitialize()
    {
        IsInitialized = false;
        UninitializeOverride();
        IsViewDisposed = true;
        DeferContentLoader = default;
    }

    [Command("LoadCommand")]
    protected virtual async Task LoadAsync()
    {
        try
        {
            // ConfigureAwait(true) sets value on UI thread
            IsInitialized = await LoadOverrideAsync().ConfigureAwait(true);
            Initialization.TrySetResult(IsInitialized);
        }
        catch (OperationCanceledException)
        {
        }
    }

    protected virtual ValueTask<bool> LoadOverrideAsync()
    {
        return ValueTask.FromResult(true);
    }

    protected virtual void UninitializeOverride()
    {
    }

    protected async ValueTask<IDisposable> EnterCriticalSectionAsync()
    {
        ThrowIfViewDisposed();
        IDisposable disposable = await DisposeLock.EnterAsync(CancellationToken).ConfigureAwait(false);
        ThrowIfViewDisposed();
        return disposable;
    }

    #region SetProperty

    protected new bool SetProperty<T>([NotNullIfNotNull(nameof(newValue))] ref T field, T newValue, [CallerMemberName] string? propertyName = null)
    {
        return !IsViewDisposed && base.SetProperty(ref field, newValue, propertyName);
    }

    protected new bool SetProperty<T>([NotNullIfNotNull(nameof(newValue))] ref T field, T newValue, IEqualityComparer<T> comparer, [CallerMemberName] string? propertyName = null)
    {
        return !IsViewDisposed && base.SetProperty(ref field, newValue, comparer, propertyName);
    }

    protected new bool SetProperty<T>(T oldValue, T newValue, Action<T> callback, [CallerMemberName] string? propertyName = null)
    {
        return !IsViewDisposed && base.SetProperty(oldValue, newValue, callback, propertyName);
    }

    protected new bool SetProperty<T>(T oldValue, T newValue, IEqualityComparer<T> comparer, Action<T> callback, [CallerMemberName] string? propertyName = null)
    {
        return !IsViewDisposed && base.SetProperty(oldValue, newValue, comparer, callback, propertyName);
    }

    protected new bool SetProperty<TModel, T>(T oldValue, T newValue, TModel model, Action<TModel, T> callback, [CallerMemberName] string? propertyName = null)
        where TModel : class
    {
        return !IsViewDisposed && base.SetProperty(oldValue, newValue, model, callback, propertyName);
    }

    protected new bool SetProperty<TModel, T>(T oldValue, T newValue, IEqualityComparer<T> comparer, TModel model, Action<TModel, T> callback, [CallerMemberName] string? propertyName = null)
        where TModel : class
    {
        return !IsViewDisposed && base.SetProperty(oldValue, newValue, comparer, model, callback, propertyName);
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
    #endregion

    private void ThrowIfViewDisposed()
    {
        if (IsViewDisposed)
        {
            HutaoException.OperationCanceled(SH.ViewModelViewDisposedOperationCancel);
        }
    }
}