// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Snap.Hutao.Core.DependencyInjection.Abstraction;
using Snap.Hutao.Core.Logging;
using Snap.Hutao.Factory.ContentDialog;
using Snap.Hutao.Service.Geetest;
using Snap.Hutao.Service.Notification;
using Snap.Hutao.Web.Hoyolab.Passport;
using Snap.Hutao.Web.Response;
using System.Runtime.CompilerServices;
using Windows.System;

namespace Snap.Hutao.UI.Xaml.View.Dialog;

internal sealed partial class UserAccountVerificationDialog : ContentDialog, IAigisProvider, INotifyPropertyChanged
{
    private readonly IContentDialogFactory contentDialogFactory;
    private readonly IServiceProvider serviceProvider;
    private readonly IGeetestService geetestService;
    private readonly ITaskContext taskContext;
    private readonly IMessenger messenger;

    private string? ticket;
    private bool isOversea;

    [GeneratedConstructor(InitializeComponent = true)]
    public partial UserAccountVerificationDialog(IServiceProvider serviceProvider);

    public event PropertyChangedEventHandler? PropertyChanged;

    public string? Email { get; set => SetProperty(ref field, value); }

    public string? Captcha
    {
        get;
        set
        {
            if (SetProperty(ref field, value))
            {
                IsLoginEnabled = !string.IsNullOrEmpty(value) && value.Length is 6;
                OnPropertyChanged(nameof(IsLoginEnabled));
            }
        }
    }

    public bool IsLoginEnabled { get; private set; }

    public string? Aigis { get; set; }

    public async ValueTask<bool> TryValidateAsync(string ticket, bool isOversea)
    {
        this.ticket = ticket;
        this.isOversea = isOversea;

        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            IPassportClient passportClient = scope.ServiceProvider.GetRequiredService<IOverseaSupportFactory<IPassportClient>>().Create(isOversea);
            Response<ActionTicketInfo> resp = await passportClient.GetActionTicketInfoAsync(ticket).ConfigureAwait(false);
            if (!ResponseValidator.TryValidate(resp, serviceProvider, out ActionTicketInfo? actionTicketInfo))
            {
                return false;
            }

            taskContext.InvokeOnMainThread(() => Email = actionTicketInfo.UserInfo.Email);
            messenger.Send(InfoBarMessage.Information(SH.ViewDialogUserAccountVerificationEmailCaptchaSent));

            ContentDialogResult result = await contentDialogFactory.EnqueueAndShowAsync(this).ShowTask.ConfigureAwait(false);
            if (result is not ContentDialogResult.Primary)
            {
                return false;
            }

            ArgumentNullException.ThrowIfNull(Captcha);

            Response resp1 = await passportClient.VerifyActionTicketPartlyAsync(ticket, Captcha).ConfigureAwait(false);
            if (!ResponseValidator.TryValidate(resp1, serviceProvider))
            {
                return false;
            }

            Response<ActionTicketInfo> resp2 = await passportClient.GetActionTicketInfoAsync(ticket).ConfigureAwait(false);
            if (!ResponseValidator.TryValidate(resp2, serviceProvider, out actionTicketInfo))
            {
                return false;
            }

            return actionTicketInfo.VerifyInfo.Status is VerifyStatus.StatusVerified;
        }
    }

    [Command("SendEmailCaptchaCommand")]
    private async Task SendEmailCaptchaAsync()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Send captcha", "UserAccountVerificationDialog.Command"));

        ArgumentNullException.ThrowIfNull(ticket);

        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            IPassportClient passportClient = scope.ServiceProvider.GetRequiredService<IOverseaSupportFactory<IPassportClient>>().Create(isOversea);
            (string? rawSession, Response response) = await passportClient.CreateEmailCaptchaByActionTicketAsync(ticket, Aigis).ConfigureAwait(false);

            if (await geetestService.TryVerifyAigisSessionAsync(this, rawSession, isOversea).ConfigureAwait(false))
            {
                (_, response) = await passportClient.CreateEmailCaptchaByActionTicketAsync(ticket, Aigis).ConfigureAwait(false);
            }

            if (ResponseValidator.TryValidate(response, serviceProvider))
            {
                messenger.Send(InfoBarMessage.Information(SH.ViewDialogUserAccountVerificationEmailCaptchaSent));
                await Task.Delay(TimeSpan.FromSeconds(10)).ConfigureAwait(false);
            }
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