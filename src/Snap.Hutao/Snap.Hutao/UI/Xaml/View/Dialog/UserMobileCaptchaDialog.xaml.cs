// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Snap.Hutao.Core.DependencyInjection.Abstraction;
using Snap.Hutao.Web.Hoyolab.Passport;
using Snap.Hutao.Web.Response;
using System.Text.RegularExpressions;
using Windows.System;

namespace Snap.Hutao.UI.Xaml.View.Dialog;

[INotifyPropertyChanged]
internal sealed partial class UserMobileCaptchaDialog : ContentDialog, IPassportMobileCaptchaProvider
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

    public string? Aigis { get; set; }

    [Command("SendMobileCaptchaCommand")]
    public async Task SendMobileCaptchaAsync()
    {
        ArgumentNullException.ThrowIfNull(Mobile);

        string? rawSession;
        Response<MobileCaptcha> response;

        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            IPassportClient passportClient = scope.ServiceProvider.GetRequiredService<IOverseaSupportFactory<IPassportClient>>().Create(false);
            (rawSession, response) = await passportClient.CreateLoginCaptchaAsync(Mobile, null).ConfigureAwait(false);
        }

        if (await this.TryResolveAigisAsync(rawSession, false, taskContext).ConfigureAwait(false))
        {
            using (IServiceScope scope = serviceProvider.CreateScope())
            {
                IPassportClient passportClient = scope.ServiceProvider.GetRequiredService<IOverseaSupportFactory<IPassportClient>>().Create(false);
                (rawSession, response) = await passportClient.CreateLoginCaptchaAsync(Mobile, Aigis).ConfigureAwait(false);
            }
        }

        if (ResponseValidator.TryValidate(response, serviceProvider, out MobileCaptcha? mobileCaptcha))
        {
            ActionType = mobileCaptcha.ActionType;
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