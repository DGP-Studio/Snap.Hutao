// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.WinUI.UI;
using Snap.Hutao.Control.Cancellable;
using Snap.Hutao.Factory.Abstraction;
using Snap.Hutao.Model.Metadata.Achievement;
using Snap.Hutao.Service.Metadata;
using System.Collections.Generic;

namespace Snap.Hutao.ViewModel;

/// <summary>
/// 成就视图模型
/// </summary>
[Injection(InjectAs.Transient)]
internal class AchievementViewModel : ObservableObject, ISupportCancellation
{
    private readonly IMetadataService metadataService;
    private AdvancedCollectionView? achievements;
    private IList<AchievementGoal>? achievementGoals;
    private AchievementGoal? selectedAchievementGoal;

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
    public AdvancedCollectionView? Achievements
    {
        get => achievements;
        set => SetProperty(ref achievements, value);
    }

    /// <summary>
    /// 成就分类
    /// </summary>
    public IList<AchievementGoal>? AchievementGoals
    {
        get => achievementGoals;
        set => SetProperty(ref achievementGoals, value);
    }

    /// <summary>
    /// 选中的成就分类
    /// </summary>
    public AchievementGoal? SelectedAchievementGoal
    {
        get => selectedAchievementGoal;
        set
        {
            SetProperty(ref selectedAchievementGoal, value);
            OnGoalChanged(value);
        }
    }

    /// <summary>
    /// 打开页面命令
    /// </summary>
    public ICommand OpenUICommand { get; }

    private async Task OpenUIAsync()
    {
        if (await metadataService.InitializeAsync(CancellationToken))
        {
            Achievements = new(await metadataService.GetAchievementsAsync(CancellationToken), true);
            AchievementGoals = await metadataService.GetAchievementGoalsAsync(CancellationToken);
        }
    }

    private void OnGoalChanged(AchievementGoal? goal)
    {
        if (Achievements != null)
        {
            Achievements.Filter = goal != null
                ? ((object o) => o is Achievement achi && achi.Goal == goal.Id)
                : ((object o) => true);
        }
    }
}