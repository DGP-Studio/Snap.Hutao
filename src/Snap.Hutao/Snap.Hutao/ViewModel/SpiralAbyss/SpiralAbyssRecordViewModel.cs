// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.Messaging;
using Snap.Hutao.Message;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Model.Metadata;
using Snap.Hutao.Model.Primitive;
using Snap.Hutao.Service.Metadata;
using Snap.Hutao.Service.Notification;
using Snap.Hutao.Service.SpiralAbyss;
using Snap.Hutao.Service.User;
using Snap.Hutao.ViewModel.User;
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
    private readonly IMetadataService metadataService;
    private readonly IInfoBarService infoBarService;
    private readonly ITaskContext taskContext;
    private readonly IUserService userService;

    private Dictionary<AvatarId, Model.Metadata.Avatar.Avatar>? idAvatarMap;
    private ObservableCollection<SpiralAbyssEntry>? spiralAbyssEntries;
    private SpiralAbyssEntry? selectedEntry;
    private SpiralAbyssView? spiralAbyssView;

    /// <summary>
    /// 深渊记录
    /// </summary>
    public ObservableCollection<SpiralAbyssEntry>? SpiralAbyssEntries { get => spiralAbyssEntries; set => SetProperty(ref spiralAbyssEntries, value); }

    /// <summary>
    /// 选中的深渊信息
    /// </summary>
    public SpiralAbyssEntry? SelectedEntry
    {
        get => selectedEntry; set
        {
            // We dont need to check the result here,
            // just refresh the view anyway.
            SetProperty(ref selectedEntry, value);
            if (value is not null && idAvatarMap is not null)
            {
                SpiralAbyssView = new(value.SpiralAbyss, idAvatarMap);
            }
        }
    }

    /// <summary>
    /// 深渊的只读视图
    /// </summary>
    public SpiralAbyssView? SpiralAbyssView { get => spiralAbyssView; set => SetProperty(ref spiralAbyssView, value); }

    /// <inheritdoc/>
    public void Receive(UserChangedMessage message)
    {
        if (UserAndUid.TryFromUser(message.NewValue, out UserAndUid? userAndUid))
        {
            UpdateSpiralAbyssCollectionAsync(userAndUid).SafeForget();
        }
        else
        {
            SpiralAbyssView = null;
        }
    }

    protected override async ValueTask<bool> InitializeUIAsync()
    {
        if (await metadataService.InitializeAsync().ConfigureAwait(false))
        {
            idAvatarMap = await metadataService.GetIdToAvatarMapAsync().ConfigureAwait(false);
            idAvatarMap = AvatarIds.WithPlayers(idAvatarMap);

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
        ObservableCollection<SpiralAbyssEntry>? temp = null;
        try
        {
            using (await EnterCriticalExecutionAsync().ConfigureAwait(false))
            {
                temp = await spiralAbyssRecordService
                    .GetSpiralAbyssCollectionAsync(userAndUid)
                    .ConfigureAwait(false);
            }
        }
        catch (OperationCanceledException)
        {
        }

        await taskContext.SwitchToMainThreadAsync();
        SpiralAbyssEntries = temp;
        SelectedEntry = SpiralAbyssEntries?.FirstOrDefault();
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
                SelectedEntry = SpiralAbyssEntries.FirstOrDefault();
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
                    infoBarService.Success(response.Message);
                }
                else
                {
                    infoBarService.Warning(response.Message);
                }
            }
        }
        else
        {
            infoBarService.Warning(SH.MustSelectUserAndUid);
        }
    }
}