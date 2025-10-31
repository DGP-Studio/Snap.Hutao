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
[DependencyProperty<string>("NewUserName")]
[DependencyProperty<string>("VerifyCode")]
[DependencyProperty<string>("NewVerifyCode")]
internal sealed partial class HutaoPassportResetUsernameDialog : ContentDialog
{
    private readonly IContentDialogFactory contentDialogFactory;
    private readonly IServiceScopeFactory serviceScopeFactory;
    private readonly IMessenger messenger;

    [GeneratedConstructor(InitializeComponent = true)]
    public partial HutaoPassportResetUsernameDialog(IServiceProvider serviceProvider);

    public async ValueTask<ValueResult<bool, (string? UserName, string? NewUserName, string? VerifyCode, string? NewVerifyCode)>> GetInputAsync(string? userName)
    {
        InitializeUserNameTextBox(userName);
        ContentDialogResult result = await contentDialogFactory.EnqueueAndShowAsync(this).ShowTask.ConfigureAwait(false);
        await contentDialogFactory.TaskContext.SwitchToMainThreadAsync();
        return new(result is ContentDialogResult.Primary, (UserName, NewUserName, VerifyCode, NewVerifyCode));
    }

    [Command("VerifyOldCommand")]
    private async Task VerifyOldAsync()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Verify old username", "HutaoPassportResetUsernameDialog.Command"));
        await PrivateVerifyAsync(UserName, VerifyCodeRequestType.ResetUserName).ConfigureAwait(false);
    }

    [Command("VerifyNewCommand")]
    private async Task VerifyNewAsync()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Verify new username", "HutaoPassportResetUsernameDialog.Command"));
        await PrivateVerifyAsync(NewUserName, VerifyCodeRequestType.ResetUserNameNew).ConfigureAwait(false);
    }

    private async ValueTask PrivateVerifyAsync(string? userName, VerifyCodeRequestType type)
    {
        if (string.IsNullOrEmpty(userName))
        {
            return;
        }

        if (!userName.IsEmail())
        {
            messenger.Send(InfoBarMessage.Warning(SH.ViewModelHutaoPassportEmailNotValidHint));
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