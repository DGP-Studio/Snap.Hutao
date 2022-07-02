// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.WinUI.UI;
using Microsoft.VisualStudio.Threading;
using Snap.Hutao.Control.Cancellable;
using Snap.Hutao.Factory.Abstraction;
using Snap.Hutao.Model.Metadata.Achievement;
using Snap.Hutao.Service.Metadata;
using System.Collections.Generic;
using System.Linq;

namespace Snap.Hutao.ViewModel;

/// <summary>
/// 成就视图模型
/// </summary>
[Injection(InjectAs.Transient)]
internal class AchievementViewModel : ObservableObject, ISupportCancellation
{
    private readonly IMetadataService metadataService;
    private AdvancedCollectionView? achievementsView;

    /// <summary>
    /// 构造一个新的成就视图模型
    /// </summary>
    /// <param name="metadataService">元数据服务</param>
    /// <param name="asyncRelayCommandFactory">异步命令工厂</param>
    public AchievementViewModel(IMetadataService metadataService, IAsyncRelayCommandFactory asyncRelayCommandFactory)
    {
        this.metadataService = metadataService;
        OpenUICommand = asyncRelayCommandFactory.Create(OpenUIAsync);
    }

    /// <inheritdoc/>
    public CancellationToken CancellationToken { get; set; }

    /// <summary>
    /// 成就视图
    /// </summary>
    public AdvancedCollectionView? AchievementsView
    {
        get => achievementsView;
        set => SetProperty(ref achievementsView, value);
    }

    /// <summary>
    /// 打开页面命令
    /// </summary>
    public ICommand OpenUICommand { get; }

    private async Task OpenUIAsync(CancellationToken token)
    {
        using (CancellationTokenExtensions.CombinedCancellationToken combined = token.CombineWith(CancellationToken))
        {
            if (await metadataService.InitializeAsync(combined.Token))
            {
                IEnumerable<Achievement> achievements = await metadataService.GetAchievementsAsync(combined.Token);

                // TODO
                AchievementsView = new(achievements.ToList());
            }
        }
    }
}
