// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Common;
using Microsoft.UI.Xaml.Controls;
using Snap.Hutao.Service.Notification;
using Snap.Hutao.Web.Hutao;
using Snap.Hutao.Web.Hutao.Response;

namespace Snap.Hutao.UI.Xaml.View.Dialog;

[DependencyProperty("UserName", typeof(string))]
[DependencyProperty("Password", typeof(string))]
[DependencyProperty("VerifyCode", typeof(string))]
internal sealed partial class HutaoPassportUnregisterDialog : ContentDialog
{
    private readonly IServiceScopeFactory serviceScopeFactory;
    private readonly IInfoBarService infoBarService;
    private readonly ITaskContext taskContext;

    public HutaoPassportUnregisterDialog(IServiceProvider serviceProvider)
    {
        InitializeComponent();

        taskContext = serviceProvider.GetRequiredService<ITaskContext>();
        serviceScopeFactory = serviceProvider.GetRequiredService<IServiceScopeFactory>();
        infoBarService = serviceProvider.GetRequiredService<IInfoBarService>();
    }

    public async ValueTask<ValueResult<bool, (string UserName, string Passport, string VerifyCode)>> GetInputAsync()
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

        using (IServiceScope scope = serviceScopeFactory.CreateScope())
        {
            HutaoPassportClient hutaoPassportClient = scope.ServiceProvider.GetRequiredService<HutaoPassportClient>();

            HutaoResponse response = await hutaoPassportClient.RequestVerifyAsync(UserName, VerifyCodeRequestType.CancelRegistration).ConfigureAwait(false);
            infoBarService.Information(response.GetLocalizationMessage());
        }
    }
}