// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Common;
using Microsoft.UI.Xaml.Controls;
using Snap.Hutao.Core.Logging;
using Snap.Hutao.Factory.ContentDialog;
using Snap.Hutao.Service.Notification;
using Snap.Hutao.Web.Hutao;
using Snap.Hutao.Web.Hutao.Response;
using Snap.Hutao.Web.Response;

namespace Snap.Hutao.UI.Xaml.View.Dialog;

[DependencyProperty<string>("UserName")]
[DependencyProperty<string>("Password")]
[DependencyProperty<string>("VerifyCode")]
internal sealed partial class HutaoPassportResetPasswordDialog : ContentDialog
{
    private readonly IContentDialogFactory contentDialogFactory;
    private readonly IServiceScopeFactory serviceScopeFactory;
    private readonly IMessenger messenger;

    [GeneratedConstructor(InitializeComponent = true)]
    public partial HutaoPassportResetPasswordDialog(IServiceProvider serviceProvider);

    public async ValueTask<ValueResult<bool, (string? UserName, string? Passport, string? VerifyCode)>> GetInputAsync(string? userName)
    {
        InitializeUserNameTextBox(userName);
        ContentDialogResult result = await contentDialogFactory.EnqueueAndShowAsync(this).ShowTask.ConfigureAwait(false);
        await contentDialogFactory.TaskContext.SwitchToMainThreadAsync();
        return new(result is ContentDialogResult.Primary, (UserName, Password, VerifyCode));
    }

    [Command("VerifyCommand")]
    private async Task VerifyAsync()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Verify", "HutaoPassportResetPasswordDialog.Command"));

        if (string.IsNullOrEmpty(UserName))
        {
            return;
        }

        if (!UserName.IsEmail())
        {
            messenger.Send(InfoBarMessage.Warning(SH.ViewModelHutaoPassportEmailNotValidHint));
            return;
        }

        using (IServiceScope scope = serviceScopeFactory.CreateScope())
        {
            HutaoPassportClient hutaoPassportClient = scope.ServiceProvider.GetRequiredService<HutaoPassportClient>();

            HutaoResponse response = await hutaoPassportClient.RequestVerifyAsync(UserName, VerifyCodeRequestType.ResetPassword).ConfigureAwait(false);
            if (!ResponseValidator.TryValidate(response, scope.ServiceProvider))
            {
                return;
            }

            messenger.Send(InfoBarMessage.Information(response.GetLocalizationMessage()));
        }
    }

    private void InitializeUserNameTextBox(string? userName)
    {
        if (string.IsNullOrEmpty(userName))
        {
            return;
        }

        UserName = userName;
        UserNameTextBox.IsEnabled = false;
    }
}