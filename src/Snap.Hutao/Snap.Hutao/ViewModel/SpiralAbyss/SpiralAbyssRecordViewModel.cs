// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Snap.Hutao.Message;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Model.Metadata;
using Snap.Hutao.Model.Primitive;
using Snap.Hutao.Service.Abstraction;
using Snap.Hutao.Service.Metadata;
using Snap.Hutao.Service.SpiralAbyss;
using Snap.Hutao.Service.User;
using Snap.Hutao.ViewModel.User;
using Snap.Hutao.Web.Hutao;
using Snap.Hutao.Web.Hutao.Model.Post;
using System.Collections.ObjectModel;

namespace Snap.Hutao.ViewModel.SpiralAbyss;

/// <summary>
/// 深渊记录视图模型
/// </summary>
[HighQuality]
[Injection(InjectAs.Scoped)]
internal sealed class SpiralAbyssRecordViewModel : Abstraction.ViewModel, IRecipient<UserChangedMessage>
{
    private readonly IServiceProvider serviceProvider;
    private readonly ITaskContext taskContext;
    private readonly ISpiralAbyssRecordService spiralAbyssRecordService;
    private readonly IMetadataService metadataService;
    private readonly IUserService userService;
    private Dictionary<AvatarId, Model.Metadata.Avatar.Avatar>? idAvatarMap;

    private ObservableCollection<SpiralAbyssEntry>? spiralAbyssEntries;
    private SpiralAbyssEntry? selectedEntry;
    private SpiralAbyssView? spiralAbyssView;

    /// <summary>
    /// 构造一个新的深渊记录视图模型
    /// </summary>
    /// <param name="serviceProvider">服务提供器</param>
    /// <param name="spiralAbyssRecordService">深渊记录服务</param>
    /// <param name="metadataService">元数据服务</param>
    /// <param name="userService">用户服务</param>
    /// <param name="messenger">消息器</param>
    public SpiralAbyssRecordViewModel(IServiceProvider serviceProvider)
    {
        taskContext = serviceProvider.GetRequiredService<ITaskContext>();
        spiralAbyssRecordService = serviceProvider.GetRequiredService<ISpiralAbyssRecordService>();
        metadataService = serviceProvider.GetRequiredService<IMetadataService>();
        userService = serviceProvider.GetRequiredService<IUserService>();
        this.serviceProvider = serviceProvider;

        RefreshCommand = new AsyncRelayCommand(RefreshAsync);
        UploadSpiralAbyssRecordCommand = new AsyncRelayCommand(UploadSpiralAbyssRecordAsync);

        serviceProvider.GetRequiredService<IMessenger>().Register(this);
    }

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
            if (value != null && idAvatarMap != null)
            {
                SpiralAbyssView = new(value.SpiralAbyss, idAvatarMap);
            }
        }
    }

    /// <summary>
    /// 深渊的只读视图
    /// </summary>
    public SpiralAbyssView? SpiralAbyssView { get => spiralAbyssView; set => SetProperty(ref spiralAbyssView, value); }

    /// <summary>
    /// 刷新界面命令
    /// </summary>
    public ICommand RefreshCommand { get; }

    /// <summary>
    /// 上传深渊记录命令
    /// </summary>
    public ICommand UploadSpiralAbyssRecordCommand { get; }

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

    /// <inheritdoc/>
    protected override async Task OpenUIAsync()
    {
        if (await metadataService.InitializeAsync().ConfigureAwait(false))
        {
            idAvatarMap = await metadataService.GetIdToAvatarMapAsync().ConfigureAwait(false);
            idAvatarMap = AvatarIds.InsertPlayers(idAvatarMap);

            if (UserAndUid.TryFromUser(userService.Current, out UserAndUid? userAndUid))
            {
                await UpdateSpiralAbyssCollectionAsync(userAndUid).ConfigureAwait(false);
                await taskContext.SwitchToMainThreadAsync();
                IsInitialized = true;
            }
            else
            {
                serviceProvider.GetRequiredService<IInfoBarService>().Warning(SH.MustSelectUserAndUid);
            }
        }
    }

    private async Task UpdateSpiralAbyssCollectionAsync(UserAndUid userAndUid)
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

    private async Task RefreshAsync()
    {
        if (SpiralAbyssEntries != null)
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

    private async Task UploadSpiralAbyssRecordAsync()
    {
        HomaSpiralAbyssClient homaClient = serviceProvider.GetRequiredService<HomaSpiralAbyssClient>();
        IInfoBarService infoBarService = serviceProvider.GetRequiredService<IInfoBarService>();

        if (UserAndUid.TryFromUser(userService.Current, out UserAndUid? userAndUid))
        {
            SimpleRecord? record = await homaClient.GetPlayerRecordAsync(userAndUid).ConfigureAwait(false);
            if (record != null)
            {
                Web.Response.Response<string> response = await homaClient.UploadRecordAsync(record).ConfigureAwait(false);

                if (response.IsOk())
                {
                    infoBarService.Success(response.Message);
                }
            }
        }
        else
        {
            infoBarService.Warning(SH.MustSelectUserAndUid);
        }
    }
}