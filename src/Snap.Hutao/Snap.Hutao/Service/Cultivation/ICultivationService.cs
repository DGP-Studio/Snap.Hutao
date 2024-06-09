// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Entity;
using Snap.Hutao.Model.Entity.Primitive;
using Snap.Hutao.ViewModel.Cultivation;
using Snap.Hutao.Web.Hoyolab.Takumi.Event.Calculate;
using System.Collections.ObjectModel;

namespace Snap.Hutao.Service.Cultivation;

/// <summary>
/// 养成计算服务
/// </summary>
[HighQuality]
internal interface ICultivationService
{
    /// <summary>
    /// 当前养成计划
    /// </summary>
    CultivateProject? Current { get; set; }

    /// <summary>
    /// 获取用于绑定的项目集合
    /// </summary>
    ObservableCollection<CultivateProject> ProjectCollection { get; }

    ValueTask<ObservableCollection<CultivateEntryView>> GetCultivateEntriesAsync(CultivateProject cultivateProject, ICultivationMetadataContext context);

    List<InventoryItemView> GetInventoryItemViews(CultivateProject cultivateProject, ICultivationMetadataContext context, ICommand saveCommand);

    ValueTask<ObservableCollection<StatisticsCultivateItem>> GetStatisticsCultivateItemCollectionAsync(
        CultivateProject cultivateProject, ICultivationMetadataContext context, CancellationToken token);

    /// <summary>
    /// 删除养成清单
    /// </summary>
    /// <param name="entryId">入口Id</param>
    /// <returns>任务</returns>
    ValueTask RemoveCultivateEntryAsync(Guid entryId);

    /// <summary>
    /// 异步移除项目
    /// </summary>
    /// <param name="project">项目</param>
    /// <returns>任务</returns>
    ValueTask RemoveProjectAsync(CultivateProject project);

    ValueTask<bool> SaveConsumptionAsync(CultivateType type, uint itemId, List<Item> items, LevelInformation levelInformation);

    /// <summary>
    /// 保存养成物品状态
    /// </summary>
    /// <param name="item">养成物品</param>
    void SaveCultivateItem(CultivateItemView item);

    /// <summary>
    /// 保存单个物品
    /// </summary>
    /// <param name="item">物品</param>
    void SaveInventoryItem(InventoryItemView item);

    /// <summary>
    /// 异步尝试添加新的项目
    /// </summary>
    /// <param name="project">项目</param>
    /// <returns>添加操作的结果</returns>
    ValueTask<ProjectAddResultKind> TryAddProjectAsync(CultivateProject project);

    ValueTask RefreshInventoryAsync(CultivateProject project);
}