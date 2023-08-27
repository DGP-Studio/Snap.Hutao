// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Binding.Hutao;
using Snap.Hutao.Service.Hutao;
using Snap.Hutao.Web.Hutao.SpiralAbyss;

namespace Snap.Hutao.ViewModel.Complex;

/// <summary>
/// 胡桃数据库视图模型
/// </summary>
[HighQuality]
[ConstructorGenerated]
[Injection(InjectAs.Scoped)]
internal sealed partial class HutaoDatabaseViewModel : Abstraction.ViewModel
{
    private readonly IHutaoCache hutaoCache;
    private readonly ITaskContext taskContext;

    private List<AvatarRankView>? avatarUsageRanks;
    private List<AvatarRankView>? avatarAppearanceRanks;
    private List<AvatarConstellationInfoView>? avatarConstellationInfos;
    private List<TeamAppearanceView>? teamAppearances;
    private Overview? overview;

    /// <summary>
    /// 角色使用率
    /// </summary>
    public List<AvatarRankView>? AvatarUsageRanks { get => avatarUsageRanks; set => SetProperty(ref avatarUsageRanks, value); }

    /// <summary>
    /// 角色上场率
    /// </summary>
    public List<AvatarRankView>? AvatarAppearanceRanks { get => avatarAppearanceRanks; set => SetProperty(ref avatarAppearanceRanks, value); }

    /// <summary>
    /// 角色命座信息
    /// </summary>
    public List<AvatarConstellationInfoView>? AvatarConstellationInfos { get => avatarConstellationInfos; set => SetProperty(ref avatarConstellationInfos, value); }

    /// <summary>
    /// 队伍出场
    /// </summary>
    public List<TeamAppearanceView>? TeamAppearances { get => teamAppearances; set => SetProperty(ref teamAppearances, value); }

    /// <summary>
    /// 总览数据
    /// </summary>
    public Overview? Overview { get => overview; set => SetProperty(ref overview, value); }

    /// <inheritdoc/>
    protected override async Task OpenUIAsync()
    {
        if (await hutaoCache.InitializeForDatabaseViewModelAsync().ConfigureAwait(false))
        {
            await taskContext.SwitchToMainThreadAsync();
            AvatarAppearanceRanks = hutaoCache.AvatarAppearanceRanks;
            AvatarUsageRanks = hutaoCache.AvatarUsageRanks;
            AvatarConstellationInfos = hutaoCache.AvatarConstellationInfos;
            TeamAppearances = hutaoCache.TeamAppearances;
            Overview = hutaoCache.Overview;
        }
    }
}
