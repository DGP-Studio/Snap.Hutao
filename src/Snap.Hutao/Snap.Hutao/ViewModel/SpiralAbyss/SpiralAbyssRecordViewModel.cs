// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.Messaging;
using Snap.Hutao.Message;
using Snap.Hutao.Service.Notification;
using Snap.Hutao.Service.SpiralAbyss;
using Snap.Hutao.Service.User;
using Snap.Hutao.ViewModel.Complex;
using Snap.Hutao.ViewModel.User;
using Snap.Hutao.Web.Hutao;
using Snap.Hutao.Web.Hutao.SpiralAbyss;
using Snap.Hutao.Web.Hutao.SpiralAbyss.Post;
using System.Collections.ObjectModel;

namespace Snap.Hutao.ViewModel.SpiralAbyss;

/// <summary>
/// 深渊记录视图模型
/// </summary>
[HighQuality]
[ConstructorGenerated]
[Injection(InjectAs.Scoped)]
internal sealed partial class SpiralAbyssRecordViewModel : Abstraction.ViewModel, IRecipient<UserChangedMessage>
{
    private readonly ISpiralAbyssRecordService spiralAbyssRecordService;
    private readonly HomaSpiralAbyssClient spiralAbyssClient;
    private readonly IInfoBarService infoBarService;
    private readonly ITaskContext taskContext;
    private readonly IUserService userService;
    private readonly HutaoDatabaseViewModel hutaoDatabaseViewModel;

    private ObservableCollection<SpiralAbyssView>? spiralAbyssEntries;
    private SpiralAbyssView? selectedView;

    /// <summary>
    /// 深渊记录
    /// </summary>
    public ObservableCollection<SpiralAbyssView>? SpiralAbyssEntries { get => spiralAbyssEntries; set => SetProperty(ref spiralAbyssEntries, value); }

    /// <summary>
    /// 选中的深渊信息
    /// </summary>
    public SpiralAbyssView? SelectedView { get => selectedView; set => SetProperty(ref selectedView, value); }

    public HutaoDatabaseViewModel HutaoDatabaseViewModel { get => hutaoDatabaseViewModel; }

    /// <inheritdoc/>
    public void Receive(UserChangedMessage message)
    {
        if (UserAndUid.TryFromUser(message.NewValue, out UserAndUid? userAndUid))
        {
            UpdateSpiralAbyssCollectionAsync(userAndUid).SafeForget();
        }
        else
        {
            SelectedView = null;
        }
    }

    protected override async ValueTask<bool> InitializeUIAsync()
    {
        if (await spiralAbyssRecordService.InitializeAsync().ConfigureAwait(false))
        {
            if (UserAndUid.TryFromUser(userService.Current, out UserAndUid? userAndUid))
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
        ObservableCollection<SpiralAbyssView>? collection = null;
        try
        {
            using (await EnterCriticalExecutionAsync().ConfigureAwait(false))
            {
                collection = await spiralAbyssRecordService
                    .GetSpiralAbyssViewCollectionAsync(userAndUid)
                    .ConfigureAwait(false);
            }
        }
        catch (OperationCanceledException)
        {
        }

        await taskContext.SwitchToMainThreadAsync();
        SpiralAbyssEntries = collection;
        SelectedView = SpiralAbyssEntries?.FirstOrDefault(s => s.Engaged);
    }

    [Command("RefreshCommand")]
    private async Task RefreshAsync()
    {
        if (SpiralAbyssEntries is not null)
        {
            if (UserAndUid.TryFromUser(userService.Current, out UserAndUid? userAndUid))
            {
                try
                {
                    using (await EnterCriticalExecutionAsync().ConfigureAwait(false))
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
                SelectedView = SpiralAbyssEntries.FirstOrDefault(s => s.Engaged);
            }
        }
    }

    [Command("UploadSpiralAbyssRecordCommand")]
    private async Task UploadSpiralAbyssRecordAsync()
    {
        if (UserAndUid.TryFromUser(userService.Current, out UserAndUid? userAndUid))
        {
            SimpleRecord? record = await spiralAbyssClient.GetPlayerRecordAsync(userAndUid).ConfigureAwait(false);
            if (record is not null)
            {
                Web.Response.Response response = await spiralAbyssClient.UploadRecordAsync(record).ConfigureAwait(false);

                if (response is { ReturnCode: 0 })
                {
                    if (response is ILocalizableResponse localizableResponse)
                    {
                        infoBarService.Success(localizableResponse.GetLocalizationMessage());
                    }
                }
                else
                {
                    if (response is ILocalizableResponse localizableResponse)
                    {
                        infoBarService.Warning(localizableResponse.GetLocalizationMessage());
                    }
                }
            }
        }
        else
        {
            infoBarService.Warning(SH.MustSelectUserAndUid);
        }
    }
}