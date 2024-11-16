// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Common;
using Microsoft.UI.Xaml.Controls;
using Snap.Hutao.Factory.ContentDialog;
using Snap.Hutao.Service.Notification;
using Snap.Hutao.Web.Hutao;
using Snap.Hutao.Web.Hutao.Response;

namespace Snap.Hutao.UI.Xaml.View.Dialog;

[ConstructorGenerated(InitializeComponent = true)]
[DependencyProperty("UserName", typeof(string))]
[DependencyProperty("Password", typeof(string))]
[DependencyProperty("VerifyCode", typeof(string))]
internal sealed partial class HutaoPassportUnregisterDialog : ContentDialog
{
    private readonly IContentDialogFactory contentDialogFactory;
    private readonly IServiceScopeFactory serviceScopeFactory;
    private readonly IInfoBarService infoBarService;

    public async ValueTask<ValueResult<bool, (string UserName, string Passport, string VerifyCode)>> GetInputAsync()
    {
        ContentDialogResult result = await contentDialogFactory.EnqueueAndShowAsync(this).ShowTask.ConfigureAwait(false);
        await contentDialogFactory.TaskContext.SwitchToMainThreadAsync();
        return new(result is ContentDialogResult.Primary, (UserName, Password, VerifyCode));
    }

    [Command("VerifyCommand")]
    private async Task VerifyAsync()
    {
        if (string.IsNullOrEmpty(UserName))
        {
            return;
        }

        if (!UserName.IsEmail())
        {
            infoBarService.Warning(SH.ViewModelHutaoPassportEmailNotValidHint);
            return;
        }

        using (IServiceScope scope = serviceScopeFactory.CreateScope())
        {
            HutaoPassportClient hutaoPassportClient = scope.ServiceProvider.GetRequiredService<HutaoPassportClient>();

            HutaoResponse response = await hutaoPassportClient.RequestVerifyAsync(UserName, VerifyCodeRequestType.CancelRegistration).ConfigureAwait(false);
            infoBarService.Information(response.GetLocalizationMessage());
        }
    }
}