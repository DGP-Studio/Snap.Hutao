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

[ConstructorGenerated(InitializeComponent = true)]
[DependencyProperty<string>("UserName")]
[DependencyProperty<string>("Password")]
[DependencyProperty<string>("VerifyCode")]
internal sealed partial class HutaoPassportRegisterDialog : ContentDialog
{
    private readonly IContentDialogFactory contentDialogFactory;
    private readonly IServiceScopeFactory serviceScopeFactory;
    private readonly IInfoBarService infoBarService;

    public async ValueTask<ValueResult<bool, (string? UserName, string? Password, string? VerifyCode)>> GetInputAsync()
    {
        ContentDialogResult result = await contentDialogFactory.EnqueueAndShowAsync(this).ShowTask.ConfigureAwait(false);
        await contentDialogFactory.TaskContext.SwitchToMainThreadAsync();
        return new(result is ContentDialogResult.Primary, (UserName, Password, VerifyCode));
    }

    [Command("VerifyCommand")]
    private async Task VerifyAsync()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Verify", "HutaoPassportRegisterDialog.Command"));

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

            HutaoResponse response = await hutaoPassportClient.RequestVerifyAsync(UserName, VerifyCodeRequestType.Registration).ConfigureAwait(false);
            if (!ResponseValidator.TryValidate(response, scope.ServiceProvider))
            {
                return;
            }

            infoBarService.Information(response.GetLocalizationMessage());
        }
    }
}