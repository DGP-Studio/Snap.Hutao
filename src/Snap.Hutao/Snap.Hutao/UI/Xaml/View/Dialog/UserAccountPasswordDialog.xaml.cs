// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Snap.Hutao.Core.DependencyInjection.Abstraction;
using Snap.Hutao.Web.Hoyolab.Passport;
using Snap.Hutao.Web.Response;
using Windows.System;

namespace Snap.Hutao.UI.Xaml.View.Dialog;

[INotifyPropertyChanged]
internal sealed partial class UserAccountPasswordDialog : ContentDialog, IPassportPasswordProvider
{
    private readonly IServiceProvider serviceProvider;
    private readonly ITaskContext taskContext;

    private string? account;
    private string? password;

    public UserAccountPasswordDialog(IServiceProvider serviceProvider)
    {
        this.serviceProvider = serviceProvider;
        taskContext = serviceProvider.GetRequiredService<ITaskContext>();
        InitializeComponent();
    }

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

    public async ValueTask<ValueResult<bool, LoginResult>> LoginAsync(bool isOversea)
    {
        await taskContext.SwitchToMainThreadAsync();
        ContentDialogResult result = await ShowAsync();
        if (result is ContentDialogResult.Primary)
        {
            LoginResult loginResult = await LoginCoreAsync(isOversea).ConfigureAwait(false);
            return new(true, loginResult);
        }

        return new(false, default!);
    }

    private async ValueTask<LoginResult> LoginCoreAsync(bool isOversea)
    {
        ArgumentNullException.ThrowIfNull(Account);
        ArgumentNullException.ThrowIfNull(Password);

        string? rawSession;
        Response<LoginResult> response;

        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            IHoyoPlayPassportClient hoyoPlayPassportClient = scope.ServiceProvider.GetRequiredService<IOverseaSupportFactory<IHoyoPlayPassportClient>>().Create(isOversea);
            (rawSession, response) = await hoyoPlayPassportClient.LoginByPasswordAsync(this).ConfigureAwait(false);
        }

        if (await this.TryResolveAigisAsync(rawSession, isOversea, taskContext).ConfigureAwait(false))
        {
            using (IServiceScope scope = serviceProvider.CreateScope())
            {
                IHoyoPlayPassportClient hoyoPlayPassportClient = scope.ServiceProvider.GetRequiredService<IOverseaSupportFactory<IHoyoPlayPassportClient>>().Create(isOversea);
                (rawSession, response) = await hoyoPlayPassportClient.LoginByPasswordAsync(this).ConfigureAwait(false);
            }
        }

        return response.IsOk() ? response.Data : default!;
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