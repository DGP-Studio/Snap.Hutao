// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Common;
using Microsoft.UI.Xaml.Controls;
using Snap.Hutao.Factory.ContentDialog;
using Snap.Hutao.Service.Notification;
using Snap.Hutao.Web.Hutao;
using Snap.Hutao.Web.Hutao.Response;
using Snap.Hutao.Web.Response;

namespace Snap.Hutao.UI.Xaml.View.Dialog;

[ConstructorGenerated(InitializeComponent = true)]
[DependencyProperty("UserName", typeof(string))]
[DependencyProperty("NewUserName", typeof(string))]
[DependencyProperty("VerifyCode", typeof(string))]
[DependencyProperty("NewVerifyCode", typeof(string))]
internal sealed partial class HutaoPassportResetUsernameDialog : ContentDialog
{
    private readonly IContentDialogFactory contentDialogFactory;
    private readonly IServiceScopeFactory serviceScopeFactory;
    private readonly IInfoBarService infoBarService;

    public async ValueTask<ValueResult<bool, (string UserName, string NewUserName, string VerifyCode, string NewVerifyCode)>> GetInputAsync(string? userName)
    {
        InitializeUserNameTextBox(userName);
        ContentDialogResult result = await contentDialogFactory.EnqueueAndShowAsync(this).ShowTask.ConfigureAwait(false);
        await contentDialogFactory.TaskContext.SwitchToMainThreadAsync();
        return new(result is ContentDialogResult.Primary, (UserName, NewUserName, VerifyCode, NewVerifyCode));
    }

    [Command("VerifyOldCommand")]
    private async Task VerifyOldAsync()
    {
        await VerifyCoreAsync(UserName, VerifyCodeRequestType.ResetUserName).ConfigureAwait(false);
    }

    [Command("VerifyNewCommand")]
    private async Task VerifyNewAsync()
    {
        await VerifyCoreAsync(NewUserName, VerifyCodeRequestType.ResetUserNameNew).ConfigureAwait(false);
    }

    private async ValueTask VerifyCoreAsync(string userName, VerifyCodeRequestType type)
    {
        if (string.IsNullOrEmpty(userName))
        {
            return;
        }

        if (!userName.IsEmail())
        {
            infoBarService.Warning(SH.ViewModelHutaoPassportEmailNotValidHint);
            return;
        }

        using (IServiceScope scope = serviceScopeFactory.CreateScope())
        {
            HutaoPassportClient hutaoPassportClient = scope.ServiceProvider.GetRequiredService<HutaoPassportClient>();

            HutaoResponse response = await hutaoPassportClient.RequestVerifyAsync(userName, type).ConfigureAwait(false);
            if (!ResponseValidator.TryValidate(response, scope.ServiceProvider))
            {
                return;
            }

            infoBarService.Information(response.GetLocalizationMessage());
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
