// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Snap.Hutao.Core.DependencyInjection.Abstraction;
using Snap.Hutao.Factory.ContentDialog;
using Snap.Hutao.Service.Geetest;
using Snap.Hutao.Web.Hoyolab.Passport;
using Snap.Hutao.Web.Response;
using System.Runtime.CompilerServices;
using Windows.System;

namespace Snap.Hutao.UI.Xaml.View.Dialog;

[ConstructorGenerated(InitializeComponent = true)]
internal sealed partial class UserAccountPasswordDialog : ContentDialog, IPassportPasswordProvider, INotifyPropertyChanged
{
    private readonly IContentDialogFactory contentDialogFactory;
    private readonly IServiceProvider serviceProvider;
    private readonly IGeetestService geetestService;

    public event PropertyChangedEventHandler? PropertyChanged;

    public string? Account { get; set => SetProperty(ref field, value); }

    public string? Password
    {
        get;
        set
        {
            if (SetProperty(ref field, value))
            {
                IsLoginEnabled = !string.IsNullOrEmpty(value);
                OnPropertyChanged(nameof(IsLoginEnabled));
            }
        }
    }

    public bool IsLoginEnabled { get; private set; }

    public string? Aigis { get; set; }

    public async ValueTask<ValueResult<bool, LoginResult?>> LoginAsync(bool isOversea)
    {
        ContentDialogResult result = await contentDialogFactory.EnqueueAndShowAsync(this).ShowTask.ConfigureAwait(false);
        if (result is ContentDialogResult.Primary)
        {
            return await PrivateLoginAsync(isOversea).ConfigureAwait(false);
        }

        return new(false, default!);
    }

    private async ValueTask<ValueResult<bool, LoginResult?>> PrivateLoginAsync(bool isOversea)
    {
        await contentDialogFactory.TaskContext.SwitchToMainThreadAsync();

        ArgumentNullException.ThrowIfNull(Account);
        ArgumentNullException.ThrowIfNull(Password);

        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            IServiceScopeIsDisposed scopeIsDisposed = scope.ServiceProvider.GetRequiredService<IServiceScopeIsDisposed>();
            IHoyoPlayPassportClient hoyoPlayPassportClient = scope.ServiceProvider.GetRequiredService<IOverseaSupportFactory<IHoyoPlayPassportClient>>().Create(isOversea);
            (string? rawSession, Response<LoginResult> response) = await hoyoPlayPassportClient.LoginByPasswordAsync(this).ConfigureAwait(false);

            if (await geetestService.TryVerifyAigisSessionAsync(this, rawSession, isOversea).ConfigureAwait(false))
            {
                (_, response) = await hoyoPlayPassportClient.LoginByPasswordAsync(this).ConfigureAwait(false);
            }

            bool ok = ResponseValidator.TryValidate(response, serviceProvider, scopeIsDisposed, out LoginResult? result);
            return new(ok, result);
        }
    }

    private void OnTextKeyDown(object sender, KeyRoutedEventArgs e)
    {
        if (e.Key is VirtualKey.Enter)
        {
            e.Handled = true;
        }
    }

    private void OnTextKeyUp(object sender, KeyRoutedEventArgs e)
    {
        if (e.Key is VirtualKey.Enter)
        {
            e.Handled = true;
        }
    }

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new(propertyName));
    }

    private bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (!EqualityComparer<T>.Default.Equals(field, value))
        {
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        return false;
    }
}