// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.WinUI.UI;
using Microsoft.Extensions.Primitives;
using Snap.Hutao.Control;
using Snap.Hutao.Extension;
using Snap.Hutao.Factory.Abstraction;
using Snap.Hutao.Message;
using Snap.Hutao.Model.Binding.Cultivation;
using Snap.Hutao.Model.Binding.Hutao;
using Snap.Hutao.Model.Binding.SpiralAbyss;
using Snap.Hutao.Model.Binding.User;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Model.Intrinsic;
using Snap.Hutao.Model.Metadata;
using Snap.Hutao.Model.Metadata.Weapon;
using Snap.Hutao.Model.Primitive;
using Snap.Hutao.Service.Abstraction;
using Snap.Hutao.Service.Cultivation;
using Snap.Hutao.Service.Hutao;
using Snap.Hutao.Service.Metadata;
using Snap.Hutao.Service.SpiralAbyss;
using Snap.Hutao.Service.User;
using Snap.Hutao.View.Dialog;
using Snap.Hutao.Web.Hutao;
using Snap.Hutao.Web.Hutao.Model.Post;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using CalcAvatarPromotionDelta = Snap.Hutao.Web.Hoyolab.Takumi.Event.Calculate.AvatarPromotionDelta;
using CalcClient = Snap.Hutao.Web.Hoyolab.Takumi.Event.Calculate.CalculateClient;
using CalcConsumption = Snap.Hutao.Web.Hoyolab.Takumi.Event.Calculate.Consumption;

namespace Snap.Hutao.ViewModel;

/// <summary>
/// 深渊记录视图模型
/// </summary>
[Injection(InjectAs.Scoped)]
internal class SpiralAbyssRecordViewModel : ObservableObject, ISupportCancellation, IRecipient<UserChangedMessage>
{
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
    /// <param name="spiralAbyssRecordService">深渊记录服务</param>
    /// <param name="metadataService">元数据服务</param>
    /// <param name="userService">用户服务</param>
    /// <param name="asyncRelayCommandFactory">异步命令工厂</param>
    /// <param name="messenger">消息器</param>
    public SpiralAbyssRecordViewModel(
        ISpiralAbyssRecordService spiralAbyssRecordService,
        IMetadataService metadataService,
        IUserService userService,
        IAsyncRelayCommandFactory asyncRelayCommandFactory,
        IMessenger messenger)
    {
        this.spiralAbyssRecordService = spiralAbyssRecordService;
        this.metadataService = metadataService;
        this.userService = userService;

        OpenUICommand = asyncRelayCommandFactory.Create(OpenUIAsync);
        RefreshCommand = asyncRelayCommandFactory.Create(RefreshAsync);
        UploadSpiralAbyssRecordCommand = asyncRelayCommandFactory.Create(UploadSpiralAbyssRecordAsync);

        messenger.Register(this);
    }

    /// <inheritdoc/>
    public CancellationToken CancellationToken { get; set; }

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
            if (SetProperty(ref selectedEntry, value))
            {
                if (value != null && idAvatarMap != null)
                {
                    SpiralAbyssView = new(value.SpiralAbyss, idAvatarMap);
                }
            }
        }
    }

    /// <summary>
    /// 深渊的只读视图
    /// </summary>
    public SpiralAbyssView? SpiralAbyssView { get => spiralAbyssView; set => SetProperty(ref spiralAbyssView, value); }

    /// <summary>
    /// 打开界面命令
    /// </summary>
    public ICommand OpenUICommand { get; }

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
        if (message.NewValue != null)
        {
            UserAndRole userAndRole = UserAndRole.FromUser(message.NewValue);
            if (userAndRole.Role != null)
            {
                UpdateSpiralAbyssCollectionAsync(UserAndRole.FromUser(message.NewValue)).SafeForget();
                return;
            }
        }

        SpiralAbyssView = null;
    }

    private async Task OpenUIAsync()
    {
        if (await metadataService.InitializeAsync().ConfigureAwait(false))
        {
            idAvatarMap = await metadataService.GetIdToAvatarMapAsync().ConfigureAwait(false);
            idAvatarMap = AvatarIds.ExtendAvatars(idAvatarMap);
            if (userService.Current?.SelectedUserGameRole != null)
            {
                await UpdateSpiralAbyssCollectionAsync(UserAndRole.FromUser(userService.Current)).ConfigureAwait(false);
            }
        }
    }

    private async Task UpdateSpiralAbyssCollectionAsync(UserAndRole userAndRole)
    {
        ObservableCollection<SpiralAbyssEntry> temp = await spiralAbyssRecordService
            .GetSpiralAbyssCollectionAsync(userAndRole)
            .ConfigureAwait(false);

        await ThreadHelper.SwitchToMainThreadAsync();
        SpiralAbyssEntries = temp;
        SelectedEntry = SpiralAbyssEntries.FirstOrDefault();
    }

    private async Task RefreshAsync()
    {
        if (await metadataService.InitializeAsync().ConfigureAwait(false))
        {
            if (userService.Current?.SelectedUserGameRole != null)
            {
                await spiralAbyssRecordService
                    .RefreshSpiralAbyssAsync(UserAndRole.FromUser(userService.Current))
                    .ConfigureAwait(false);

                await ThreadHelper.SwitchToMainThreadAsync();
                SelectedEntry = SpiralAbyssEntries?.FirstOrDefault();
            }
        }
    }

    private async Task UploadSpiralAbyssRecordAsync()
    {
        HomaClient homaClient = Ioc.Default.GetRequiredService<HomaClient>();
        IInfoBarService infoBarService = Ioc.Default.GetRequiredService<IInfoBarService>();

        if (userService.Current is Model.Binding.User.User user)
        {
            if (user.SelectedUserGameRole == null)
            {
                infoBarService.Warning("尚未选择角色");
            }

            SimpleRecord record = await homaClient.GetPlayerRecordAsync(user).ConfigureAwait(false);
            Web.Response.Response<string>? response = await homaClient.UploadRecordAsync(record).ConfigureAwait(false);

            if (response != null)
            {
                if (response.IsOk())
                {
                    infoBarService.Success(response.Message);
                }
                else
                {
                    infoBarService.Information(response.Message);
                }
            }
        }
    }
}