// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Snap.Hutao.Core.DependencyInjection.Abstraction;
using Snap.Hutao.Core.Logging;
using Snap.Hutao.Factory.ContentDialog;
using Snap.Hutao.Service.Geetest;
using Snap.Hutao.Web.Hoyolab.Passport;
using Snap.Hutao.Web.Response;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using Windows.System;

namespace Snap.Hutao.UI.Xaml.View.Dialog;

internal sealed partial class UserMobileCaptchaDialog : ContentDialog, IPassportMobileCaptchaProvider, INotifyPropertyChanged
{
    private readonly IContentDialogFactory contentDialogFactory;
    private readonly IServiceProvider serviceProvider;
    private readonly IGeetestService geetestService;

    [GeneratedConstructor(InitializeComponent = true)]
    public partial UserMobileCaptchaDialog(IServiceProvider serviceProvider);

    public event PropertyChangedEventHandler? PropertyChanged;

    public string? Mobile
    {
        get;
        set
        {
            if (SetProperty(ref field, value) && value is not null)
            {
                IsSendCaptchaEnabled = MobilePhoneRegex.IsMatch(value);
                OnPropertyChanged(nameof(IsSendCaptchaEnabled));
            }
        }
    }

    public bool IsSendCaptchaEnabled { get; private set; }

    public string? Captcha
    {
        get;
        set
        {
            if (SetProperty(ref field, value))
            {
                IsLoginEnabled = !string.IsNullOrEmpty(value) && !string.IsNullOrEmpty(ActionType);
                OnPropertyChanged(nameof(IsLoginEnabled));
            }
        }
    }

    public bool IsLoginEnabled { get; private set; }

    public string? ActionType { get; private set; }

    public string? Aigis { get; set; }

    [GeneratedRegex(@"\d{11}")]
    private static partial Regex MobilePhoneRegex { get; }

    [Command("SendMobileCaptchaCommand")]
    public async Task SendMobileCaptchaAsync()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Send captcha", "UserMobileCaptchaDialog.Command"));

        ArgumentNullException.ThrowIfNull(Mobile);

        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            IPassportClient passportClient = scope.ServiceProvider.GetRequiredService<IOverseaSupportFactory<IPassportClient>>().Create(false);
            (string? rawSession, Response<MobileCaptcha> response) = await passportClient.CreateLoginCaptchaAsync(Mobile, null).ConfigureAwait(false);

            if (await geetestService.TryVerifyAigisSessionAsync(this, rawSession, false).ConfigureAwait(false))
            {
                (_, response) = await passportClient.CreateLoginCaptchaAsync(Mobile, Aigis).ConfigureAwait(false);
            }

            try
            {
                if (ResponseValidator.TryValidate(response, scope.ServiceProvider, out MobileCaptcha? mobileCaptcha))
                {
                    ActionType = mobileCaptcha.ActionType;
                }
            }
            catch (ObjectDisposedException)
            {
                // The user close the dialog before network request finished
                return;
            }
        }

        // Prevent re-enable too soon, and user might not receive the short message
        await Task.Delay(TimeSpan.FromSeconds(10)).ConfigureAwait(false);
    }

    public async ValueTask<bool> GetMobileCaptchaAsync()
    {
        return await contentDialogFactory.EnqueueAndShowAsync(this).ShowTask.ConfigureAwait(false) is ContentDialogResult.Primary;
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