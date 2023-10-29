// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Common;
using Microsoft.UI.Xaml.Controls;
using Snap.Hutao.Service.Notification;
using Snap.Hutao.Web.Hutao;

namespace Snap.Hutao.View.Dialog;

[DependencyProperty("UserName", typeof(string))]
[DependencyProperty("Password", typeof(string))]
[DependencyProperty("VerifyCode", typeof(string))]
internal sealed partial class HutaoPassportRegisterDialog : ContentDialog
{
    private readonly HomaPassportClient homaPassportClient;
    private readonly IInfoBarService infoBarService;
    private readonly ITaskContext taskContext;

    public HutaoPassportRegisterDialog(IServiceProvider serviceProvider)
    {
        InitializeComponent();

        taskContext = serviceProvider.GetRequiredService<ITaskContext>();
        homaPassportClient = serviceProvider.GetRequiredService<HomaPassportClient>();
        infoBarService = serviceProvider.GetRequiredService<IInfoBarService>();
    }

    public async ValueTask<ValueResult<bool, (string UserName, string Password, string VerifyCode)>> GetInputAsync()
    {
        await taskContext.SwitchToMainThreadAsync();
        ContentDialogResult result = await ShowAsync();

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

        HutaoResponse response = await homaPassportClient.VerifyAsync(UserName, false).ConfigureAwait(false);
        infoBarService.Information(response.GetLocalizationMessage());
    }
}
