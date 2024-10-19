// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Snap.Hutao.Core.DependencyInjection.Abstraction;
using Snap.Hutao.Service.Geetest;
using Snap.Hutao.Web.Hoyolab.Passport;
using Snap.Hutao.Web.Response;
using System.Text.RegularExpressions;
using Windows.System;

namespace Snap.Hutao.UI.Xaml.View.Dialog;

[INotifyPropertyChanged]
[ConstructorGenerated(InitializeComponent = true)]
internal sealed partial class UserMobileCaptchaDialog : ContentDialog, IPassportMobileCaptchaProvider
{
    private readonly IServiceProvider serviceProvider;
    private readonly IGeetestService geetestService;
    private readonly ITaskContext taskContext;

    private string? mobile;
    private string? captcha;

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
                IsLoginEnabled = !string.IsNullOrEmpty(value) && !string.IsNullOrEmpty(ActionType);
                OnPropertyChanged(nameof(IsLoginEnabled));
            }
        }
    }

    public bool IsLoginEnabled { get; private set; }

    public string? ActionType { get; private set; }

    public string? Aigis { get; set; }

    [Command("SendMobileCaptchaCommand")]
    public async Task SendMobileCaptchaAsync()
    {
        ArgumentNullException.ThrowIfNull(Mobile);

        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            IPassportClient passportClient = scope.ServiceProvider.GetRequiredService<IOverseaSupportFactory<IPassportClient>>().Create(false);
            (string? rawSession, Response<MobileCaptcha> response) = await passportClient.CreateLoginCaptchaAsync(Mobile, null).ConfigureAwait(false);

            if (await geetestService.TryVerifyAigisSessionAsync(this, rawSession, false).ConfigureAwait(false))
            {
                (_, response) = await passportClient.CreateLoginCaptchaAsync(Mobile, Aigis).ConfigureAwait(false);
            }

            if (ResponseValidator.TryValidate(response, serviceProvider, out MobileCaptcha? mobileCaptcha))
            {
                ActionType = mobileCaptcha.ActionType;
            }
        }

        // Prevent re-enable too soon, and user might not receive the short message
        await Task.Delay(TimeSpan.FromSeconds(10)).ConfigureAwait(false);
    }

    public async ValueTask<bool> GetMobileCaptchaAsync()
    {
        await taskContext.SwitchToMainThreadAsync();
        return await ShowAsync() is ContentDialogResult.Primary;
    }

    [GeneratedRegex(@"\d{11}")]
    private static partial Regex MobilePhoneRegex();

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