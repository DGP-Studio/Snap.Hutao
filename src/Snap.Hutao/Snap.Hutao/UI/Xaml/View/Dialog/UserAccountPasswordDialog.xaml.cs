// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Snap.Hutao.Core.DependencyInjection.Abstraction;
using Snap.Hutao.Factory.ContentDialog;
using Snap.Hutao.Service.Geetest;
using Snap.Hutao.Web.Hoyolab.Passport;
using Snap.Hutao.Web.Response;
using Windows.System;

namespace Snap.Hutao.UI.Xaml.View.Dialog;

[INotifyPropertyChanged]
[ConstructorGenerated(InitializeComponent = true)]
internal sealed partial class UserAccountPasswordDialog : ContentDialog, IPassportPasswordProvider
{
    private readonly IContentDialogFactory contentDialogFactory;
    private readonly IServiceProvider serviceProvider;
    private readonly IGeetestService geetestService;

    private string? account;
    private string? password;

    public string? Account { get => account; set => SetProperty(ref account, value); }

    public string? Password
    {
        get => password;
        set
        {
            if (SetProperty(ref password, value))
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
            return await LoginCoreAsync(isOversea).ConfigureAwait(false);
        }

        return new(false, default!);
    }

    private async ValueTask<ValueResult<bool, LoginResult?>> LoginCoreAsync(bool isOversea)
    {
        await contentDialogFactory.TaskContext.SwitchToMainThreadAsync();

        ArgumentNullException.ThrowIfNull(Account);
        ArgumentNullException.ThrowIfNull(Password);

        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            IHoyoPlayPassportClient hoyoPlayPassportClient = scope.ServiceProvider.GetRequiredService<IOverseaSupportFactory<IHoyoPlayPassportClient>>().Create(isOversea);
            (string? rawSession, Response<LoginResult> response) = await hoyoPlayPassportClient.LoginByPasswordAsync(this).ConfigureAwait(false);

            if (await geetestService.TryVerifyAigisSessionAsync(this, rawSession, isOversea).ConfigureAwait(false))
            {
                (_, response) = await hoyoPlayPassportClient.LoginByPasswordAsync(this).ConfigureAwait(false);
            }

            bool ok = ResponseValidator.TryValidate(response, serviceProvider, out LoginResult? result);
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
}