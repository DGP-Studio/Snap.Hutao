// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Snap.Hutao.UI.Xaml.Behavior.Action;
using Snap.Hutao.UI.Xaml.View.Window.WebView2;
using Snap.Hutao.Web.Hoyolab.Passport;
using Snap.Hutao.Web.Response;
using System.Text;
using System.Text.RegularExpressions;

namespace Snap.Hutao.UI.Xaml.View.Dialog;

[INotifyPropertyChanged]
internal sealed partial class UserMobileCaptchaDialog : ContentDialog
{
    private readonly IServiceProvider serviceProvider;
    private readonly ITaskContext taskContext;

    private string? mobile;
    private string? captcha;

    public UserMobileCaptchaDialog(IServiceProvider serviceProvider)
    {
        this.serviceProvider = serviceProvider;
        taskContext = serviceProvider.GetRequiredService<ITaskContext>();
        InitializeComponent();
    }

    public string? Mobile
    {
        get => mobile;
        set
        {
            if (SetProperty(ref mobile, value) && value is not null)
            {
                IsSendCaptchaEnabled = MobilePhoneRegex().IsMatch(value);
                OnPropertyChanged(nameof(IsSendCaptchaEnabled));
            }
        }
    }

    public bool IsSendCaptchaEnabled { get; private set; }

    public string? Captcha
    {
        get => captcha;
        set
        {
            if (SetProperty(ref captcha, value))
            {
                IsLoginEnabled = !string.IsNullOrEmpty(value);
                OnPropertyChanged(nameof(IsLoginEnabled));
            }
        }
    }

    public bool IsLoginEnabled { get; private set; }

    public string? ActionType { get; private set; }

    [Command("SendMobileCaptchaCommand")]
    public async Task SendMobileCaptchaAsync()
    {
        ArgumentNullException.ThrowIfNull(Mobile);

        string? rawSession = default;
        Response<MobileCaptcha> response = default!;

        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            PassportClient2 passportClient2 = scope.ServiceProvider.GetRequiredService<PassportClient2>();
            (rawSession, response) = await passportClient2.CreateLoginCaptchaAsync(Mobile, null).ConfigureAwait(false);
        }

        if (!string.IsNullOrEmpty(rawSession))
        {
            Aigis? session = JsonSerializer.Deserialize<Aigis>(rawSession);
            ArgumentNullException.ThrowIfNull(session);
            AigisData? sessionData = JsonSerializer.Deserialize<AigisData>(session.Data);
            ArgumentNullException.ThrowIfNull(sessionData);

            await taskContext.SwitchToMainThreadAsync();
            GeetestWebView2ContentProvider contentProvider = new(sessionData.GT, sessionData.Challenge);

            new ShowWebView2WindowAction()
            {
                ContentProvider = contentProvider,
            }.ShowAt(XamlRoot);

            await taskContext.SwitchToBackgroundAsync();
            string? result = await contentProvider.GetResultAsync().ConfigureAwait(false);

            if (string.IsNullOrEmpty(result))
            {
                // User closed the window without completing the verification
                return;
            }

            string aigis = $"{session.SessionId};{Convert.ToBase64String(Encoding.UTF8.GetBytes(result))}";
            using (IServiceScope scope = serviceProvider.CreateScope())
            {
                PassportClient2 passportClient2 = scope.ServiceProvider.GetRequiredService<PassportClient2>();
                (rawSession, response) = await passportClient2.CreateLoginCaptchaAsync(Mobile, aigis).ConfigureAwait(false);
            }
        }

        if (response.IsOk())
        {
            ActionType = response.Data.ActionType;
        }

        // Prevent re-enable too soon, and user might not receive the short message
        await Task.Delay(TimeSpan.FromSeconds(10)).ConfigureAwait(false);
    }

    public async ValueTask<bool> GetMobileCaptchaAsync()
    {
        await taskContext.SwitchToMainThreadAsync();
        return await ShowAsync() is ContentDialogResult.Primary;
    }

    private static void OnMobileChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
    {
        UserMobileCaptchaDialog dialog = (UserMobileCaptchaDialog)sender;
        dialog.IsSendCaptchaEnabled = MobilePhoneRegex().IsMatch((string)args.NewValue);
    }

    private static void OnCaptchaChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
    {
        UserMobileCaptchaDialog dialog = (UserMobileCaptchaDialog)sender;
        dialog.IsLoginEnabled = !string.IsNullOrEmpty((string)args.NewValue);
    }

    [GeneratedRegex(@"\d{11}")]
    private static partial Regex MobilePhoneRegex();

    private sealed class Aigis
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