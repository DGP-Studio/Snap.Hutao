// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using JetBrains.Annotations;
using Snap.Hutao.Core.ExceptionService;
using Snap.Hutao.Core.Property;
using Snap.Hutao.UI.Xaml;
using Snap.Hutao.Win32.Foundation;
using System.Runtime.CompilerServices;

namespace Snap.Hutao.ViewModel.Abstraction;

internal abstract partial class ViewModel : ObservableObject, IViewModel, IDisposable
{
    private bool initializing;

    public bool IsInitialized { get; protected set => SetProperty(ref field, value); }

    public CancellationToken CancellationToken { get; set; }

    [field: MaybeNull]
    public SemaphoreSlim CriticalSection { get => field ??= new(1); private set; }

    public IDeferContentLoader? DeferContentLoader { get; set; }

    [field: MaybeNull]
    public IProperty<bool> IsViewUnloaded { get => field ??= Property.Create(false); protected set; }

    protected TaskCompletionSource<bool> Initialization { get; set; } = new();

    public virtual void Dispose()
    {
        CriticalSection.Dispose();
    }

    public void Resurrect()
    {
        IsViewUnloaded.Value = false;
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
        IsViewUnloaded.Value = true;
        DeferContentLoader = default;
    }

    [Command("LoadCommand")]
    protected virtual async Task LoadAsync()
    {
        try
        {
            if (Interlocked.Exchange(ref initializing, true))
            {
                await Initialization.Task.ConfigureAwait(false);
                return;
            }

            IsInitialized = await LoadOverrideAsync(CancellationToken).ConfigureAwait(true);
            Initialization.TrySetResult(IsInitialized);
        }
        catch (OperationCanceledException)
        {
        }
        finally
        {
            Volatile.Write(ref initializing, false);
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

    protected void MakeSubViewModel(ReadOnlySpan<ViewModel> subViewModels)
    {
        foreach (ViewModel subViewModel in subViewModels)
        {
            subViewModel.CancellationToken = CancellationToken;
            subViewModel.CriticalSection = CriticalSection;
            subViewModel.IsViewUnloaded = IsViewUnloaded;
        }
    }

    #region SetProperty

    [NotifyPropertyChangedInvocator]
    protected new bool SetProperty<T>([NotNullIfNotNull(nameof(newValue))] ref T field, T newValue, [CallerMemberName] string? propertyName = null)
    {
        return !IsViewUnloaded.Value && base.SetProperty(ref field, newValue, propertyName);
    }

    [NotifyPropertyChangedInvocator]
    protected new bool SetProperty<T>([NotNullIfNotNull(nameof(newValue))] ref T field, T newValue, IEqualityComparer<T> comparer, [CallerMemberName] string? propertyName = null)
    {
        return !IsViewUnloaded.Value && base.SetProperty(ref field, newValue, comparer, propertyName);
    }

    [NotifyPropertyChangedInvocator]
    protected new bool SetProperty<T>(T oldValue, T newValue, Action<T> callback, [CallerMemberName] string? propertyName = null)
    {
        return !IsViewUnloaded.Value && base.SetProperty(oldValue, newValue, callback, propertyName);
    }

    [NotifyPropertyChangedInvocator]
    protected new bool SetProperty<T>(T oldValue, T newValue, IEqualityComparer<T> comparer, Action<T> callback, [CallerMemberName] string? propertyName = null)
    {
        return !IsViewUnloaded.Value && base.SetProperty(oldValue, newValue, comparer, callback, propertyName);
    }

    [NotifyPropertyChangedInvocator]
    protected new bool SetProperty<TModel, T>(T oldValue, T newValue, TModel model, Action<TModel, T> callback, [CallerMemberName] string? propertyName = null)
        where TModel : class
    {
        return !IsViewUnloaded.Value && base.SetProperty(oldValue, newValue, model, callback, propertyName);
    }

    [NotifyPropertyChangedInvocator]
    protected new bool SetProperty<TModel, T>(T oldValue, T newValue, IEqualityComparer<T> comparer, TModel model, Action<TModel, T> callback, [CallerMemberName] string? propertyName = null)
        where TModel : class
    {
        return !IsViewUnloaded.Value && base.SetProperty(oldValue, newValue, comparer, model, callback, propertyName);
    }

    #endregion

    private void ThrowIfViewDisposed()
    {
        if (IsViewUnloaded.Value)
        {
            HutaoException.OperationCanceled(SH.ViewModelViewDisposedOperationCancel);
        }
    }
}