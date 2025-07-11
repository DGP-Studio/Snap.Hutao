// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using JetBrains.Annotations;
using Snap.Hutao.Core.ExceptionService;
using Snap.Hutao.UI.Xaml;
using Snap.Hutao.Win32.Foundation;
using System.Runtime.CompilerServices;

namespace Snap.Hutao.ViewModel.Abstraction;

internal abstract partial class ViewModel : ObservableObject, IViewModel, IDisposable
{
    public bool IsInitialized { get; protected set => SetProperty(ref field, value); }

    public CancellationToken CancellationToken { get; set; }

    public SemaphoreSlim CriticalSection { get; } = new(1);

    public IDeferContentLoader? DeferContentLoader { get; set; }

    public bool IsViewUnloaded { get; set; }

    protected TaskCompletionSource<bool> Initialization { get; set; } = new();

    public virtual void Dispose()
    {
        CriticalSection.Dispose();
    }

    public void Resurrect()
    {
        IsViewUnloaded = false;
        Initialization = new();
    }

    public void Uninitialize()
    {
        try
        {
            IsInitialized = false;
        }
        catch (Exception ex) when (ex.HResult is HRESULT.E_UNEXPECTED)
        {
            // Ignore
        }

        UninitializeOverride();
        IsViewUnloaded = true;
        DeferContentLoader = default;
    }

    [Command("LoadCommand")]
    protected virtual async Task LoadAsync()
    {
        try
        {
            IsInitialized = await LoadOverrideAsync(CancellationToken).ConfigureAwait(true);
            Initialization.TrySetResult(IsInitialized);
        }
        catch (OperationCanceledException)
        {
        }
    }

    /// <summary>
    /// Override this method to implement the loading logic.
    /// </summary>
    /// <param name="token">The same as <see cref="CancellationToken"/></param>
    /// <returns>Is the initialization successful</returns>
    protected virtual ValueTask<bool> LoadOverrideAsync(CancellationToken token)
    {
        token.ThrowIfCancellationRequested();
        return ValueTask.FromResult(true);
    }

    protected virtual void UninitializeOverride()
    {
    }

    protected async ValueTask<IDisposable> EnterCriticalSectionAsync()
    {
        ThrowIfViewDisposed();
        IDisposable disposable = await CriticalSection.EnterAsync(CancellationToken).ConfigureAwait(false);
        ThrowIfViewDisposed();
        return disposable;
    }

    #region SetProperty

    [NotifyPropertyChangedInvocator]
    protected new bool SetProperty<T>([NotNullIfNotNull(nameof(newValue))] ref T field, T newValue, [CallerMemberName] string? propertyName = null)
    {
        return !IsViewUnloaded && base.SetProperty(ref field, newValue, propertyName);
    }

    [NotifyPropertyChangedInvocator]
    protected new bool SetProperty<T>([NotNullIfNotNull(nameof(newValue))] ref T field, T newValue, IEqualityComparer<T> comparer, [CallerMemberName] string? propertyName = null)
    {
        return !IsViewUnloaded && base.SetProperty(ref field, newValue, comparer, propertyName);
    }

    [NotifyPropertyChangedInvocator]
    protected new bool SetProperty<T>(T oldValue, T newValue, Action<T> callback, [CallerMemberName] string? propertyName = null)
    {
        return !IsViewUnloaded && base.SetProperty(oldValue, newValue, callback, propertyName);
    }

    [NotifyPropertyChangedInvocator]
    protected new bool SetProperty<T>(T oldValue, T newValue, IEqualityComparer<T> comparer, Action<T> callback, [CallerMemberName] string? propertyName = null)
    {
        return !IsViewUnloaded && base.SetProperty(oldValue, newValue, comparer, callback, propertyName);
    }

    [NotifyPropertyChangedInvocator]
    protected new bool SetProperty<TModel, T>(T oldValue, T newValue, TModel model, Action<TModel, T> callback, [CallerMemberName] string? propertyName = null)
        where TModel : class
    {
        return !IsViewUnloaded && base.SetProperty(oldValue, newValue, model, callback, propertyName);
    }

    [NotifyPropertyChangedInvocator]
    protected new bool SetProperty<TModel, T>(T oldValue, T newValue, IEqualityComparer<T> comparer, TModel model, Action<TModel, T> callback, [CallerMemberName] string? propertyName = null)
        where TModel : class
    {
        return !IsViewUnloaded && base.SetProperty(oldValue, newValue, comparer, model, callback, propertyName);
    }

    #endregion

    private void ThrowIfViewDisposed()
    {
        if (IsViewUnloaded)
        {
            HutaoException.OperationCanceled(SH.ViewModelViewDisposedOperationCancel);
        }
    }
}