// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.Messaging;
using Microsoft.UI.Xaml.Controls;
using Snap.Hutao.Factory.ContentDialog;
using Snap.Hutao.Service.Hutao;
using Snap.Hutao.Service.Navigation;
using Snap.Hutao.Service.Notification;
using Snap.Hutao.Service.SpiralAbyss;
using Snap.Hutao.Service.User;
using Snap.Hutao.UI.Xaml.Data;
using Snap.Hutao.UI.Xaml.View.Dialog;
using Snap.Hutao.UI.Xaml.View.Page;
using Snap.Hutao.ViewModel.Complex;
using Snap.Hutao.ViewModel.User;
using Snap.Hutao.Web.Hutao.Response;
using Snap.Hutao.Web.Hutao.SpiralAbyss;
using System.Collections.ObjectModel;

namespace Snap.Hutao.ViewModel.SpiralAbyss;

[ConstructorGenerated]
[Injection(InjectAs.Scoped)]
internal sealed partial class SpiralAbyssRecordViewModel : Abstraction.ViewModel, IRecipient<UserAndUidChangedMessage>
{
    private readonly ISpiralAbyssRecordService spiralAbyssRecordService;
    private readonly IContentDialogFactory contentDialogFactory;
    private readonly HutaoSpiralAbyssClient spiralAbyssClient;
    private readonly INavigationService navigationService;
    private readonly IInfoBarService infoBarService;
    private readonly ITaskContext taskContext;
    private readonly IUserService userService;
    private readonly HutaoDatabaseViewModel hutaoDatabaseViewModel;
    private readonly HutaoUserOptions hutaoUserOptions;

    private AdvancedCollectionView<SpiralAbyssView>? spiralAbyssEntries;

    /// <summary>
    /// 深渊记录
    /// </summary>
    public AdvancedCollectionView<SpiralAbyssView>? SpiralAbyssEntries { get => spiralAbyssEntries; set => SetProperty(ref spiralAbyssEntries, value); }

    public HutaoDatabaseViewModel HutaoDatabaseViewModel { get => hutaoDatabaseViewModel; }

    public void Receive(UserAndUidChangedMessage message)
    {
        if (message.UserAndUid is { } userAndUid)
        {
            UpdateSpiralAbyssCollectionAsync(userAndUid).SafeForget();
        }
        else
        {
            SpiralAbyssEntries?.MoveCurrentTo(default);
        }
    }

    protected override async ValueTask<bool> InitializeOverrideAsync()
    {
        if (await spiralAbyssRecordService.InitializeAsync().ConfigureAwait(false))
        {
            if (await userService.GetCurrentUserAndUidAsync().ConfigureAwait(false) is { } userAndUid)
            {
                await UpdateSpiralAbyssCollectionAsync(userAndUid).ConfigureAwait(false);
                return true;
            }
            else
            {
                infoBarService.Warning(SH.MustSelectUserAndUid);
            }
        }

        return false;
    }

    private async ValueTask UpdateSpiralAbyssCollectionAsync(UserAndUid userAndUid)
    {
        ObservableCollection<SpiralAbyssView> collection;
        try
        {
            using (await EnterCriticalSectionAsync().ConfigureAwait(false))
            {
                collection = await spiralAbyssRecordService
                    .GetSpiralAbyssViewCollectionAsync(userAndUid)
                    .ConfigureAwait(false);
            }

            AdvancedCollectionView<SpiralAbyssView> spiralAbyssEntries = collection.ToAdvancedCollectionView();

            await taskContext.SwitchToMainThreadAsync();
            SpiralAbyssEntries = spiralAbyssEntries;
            SpiralAbyssEntries.MoveCurrentTo(SpiralAbyssEntries.SourceCollection.FirstOrDefault(s => s.Engaged));
        }
        catch (OperationCanceledException)
        {
            return;
        }
    }

    [Command("RefreshCommand")]
    private async Task RefreshAsync()
    {
        if (SpiralAbyssEntries is not null)
        {
            if (await userService.GetCurrentUserAndUidAsync().ConfigureAwait(false) is { } userAndUid)
            {
                try
                {
                    using (await EnterCriticalSectionAsync().ConfigureAwait(false))
                    {
                        await spiralAbyssRecordService
                            .RefreshSpiralAbyssAsync(userAndUid)
                            .ConfigureAwait(false);
                    }
                }
                catch (OperationCanceledException)
                {
                }

                await taskContext.SwitchToMainThreadAsync();
                SpiralAbyssEntries.MoveCurrentTo(SpiralAbyssEntries.SourceCollection.FirstOrDefault(s => s.Engaged));
            }
        }
    }

    [Command("UploadSpiralAbyssRecordCommand")]
    private async Task UploadSpiralAbyssRecordAsync()
    {
        if (await userService.GetCurrentUserAndUidAsync().ConfigureAwait(false) is { } userAndUid)
        {
            if (!hutaoUserOptions.IsLoggedIn)
            {
                SpiralAbyssUploadRecordHomaNotLoginDialog dialog = await contentDialogFactory
                    .CreateInstanceAsync<SpiralAbyssUploadRecordHomaNotLoginDialog>()
                    .ConfigureAwait(false);

                await taskContext.SwitchToMainThreadAsync();
                ContentDialogResult result = await dialog.ShowAsync();

                switch (result)
                {
                    case ContentDialogResult.Primary:
                        await navigationService.NavigateAsync<SettingPage>(INavigationAwaiter.Default, true).ConfigureAwait(false);
                        return;

                    case ContentDialogResult.Secondary:
                        break;

                    case ContentDialogResult.None:
                        return;
                }
            }

            if (await spiralAbyssClient.GetPlayerRecordAsync(userAndUid).ConfigureAwait(false) is { } record)
            {
                Web.Response.Response response = await spiralAbyssClient.UploadRecordAsync(record).ConfigureAwait(false);

                if (response is ILocalizableResponse localizableResponse)
                {
                    infoBarService.PrepareInfoBarAndShow(builder =>
                    {
                        builder
                        .SetSeverity(response is { ReturnCode: 0 } ? InfoBarSeverity.Success : InfoBarSeverity.Warning)
                        .SetMessage(localizableResponse.GetLocalizationMessage());
                    });
                }
            }
        }
        else
        {
            infoBarService.Warning(SH.MustSelectUserAndUid);
        }
    }
}