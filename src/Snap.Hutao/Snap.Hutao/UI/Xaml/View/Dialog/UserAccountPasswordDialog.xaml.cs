// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Snap.Hutao.Core.DependencyInjection.Abstraction;
using Snap.Hutao.UI.Xaml.Behavior.Action;
using Snap.Hutao.UI.Xaml.View.Window.WebView2;
using Snap.Hutao.Web.Hoyolab.Passport;
using Snap.Hutao.Web.Response;
using System.Text;
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

    public string? Aigis { get; private set; }

    public async ValueTask<ValueResult<bool, LoginResult>> LoginAsync(bool isOversea)
    {
        await taskContext.SwitchToMainThreadAsync();
        ContentDialogResult result = await ShowAsync();
        LoginResult? loginResult = await LoginCoreAsync(isOversea).ConfigureAwait(false);

        return new(result is ContentDialogResult.Primary, loginResult);
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

        if (!string.IsNullOrEmpty(rawSession))
        {
            AigisObject? session = JsonSerializer.Deserialize<AigisObject>(rawSession);
            ArgumentNullException.ThrowIfNull(session);
            AigisData? sessionData = JsonSerializer.Deserialize<AigisData>(session.Data);
            ArgumentNullException.ThrowIfNull(sessionData);

            await taskContext.SwitchToMainThreadAsync();
            GeetestWebView2ContentProvider contentProvider = new(sessionData.GT, sessionData.Challenge, isOversea);

            new ShowWebView2WindowAction()
            {
                ContentProvider = contentProvider,
            }.ShowAt(XamlRoot);

            await taskContext.SwitchToBackgroundAsync();
            string? result = await contentProvider.GetResultAsync().ConfigureAwait(false);

            if (string.IsNullOrEmpty(result))
            {
                // User closed the window without completing the verification
                return default!;
            }

            Aigis = $"{session.SessionId};{Convert.ToBase64String(Encoding.UTF8.GetBytes(result))}";
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

    private sealed class AigisObject
    {
        [JsonPropertyName("session_id")]
        public string SessionId { get; set; } = default!;

        [JsonPropertyName("mmt_type")]
        public int MmtType { get; set; } = default!;

        [JsonPropertyName("data")]
        public string Data { get; set; } = default!;
    }

    private sealed class AigisData
    {
        [JsonPropertyName("success")]
        public int Success { get; set; }

        [JsonPropertyName("gt")]
        public string GT { get; set; } = default!;

        [JsonPropertyName("challenge")]
        public string Challenge { get; set; } = default!;

        [JsonPropertyName("new_captcha")]
        public int NewCaptcha { get; set; }
    }
}