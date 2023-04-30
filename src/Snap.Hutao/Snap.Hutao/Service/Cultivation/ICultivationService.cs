// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Entity;
using Snap.Hutao.Model.Entity.Primitive;
using Snap.Hutao.Model.Metadata.Item;
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

    /// <summary>
    /// 获取绑定用的养成列表
    /// </summary>
    /// <param name="cultivateProject">养成计划</param>
    /// <returns>绑定用的养成列表</returns>
    Task<ObservableCollection<CultivateEntryView>> GetCultivateEntriesAsync(CultivateProject cultivateProject);

    /// <summary>
    /// 获取物品列表
    /// </summary>
    /// <param name="cultivateProject">养成计划</param>
    /// <param name="metadata">元数据</param>
    /// <param name="saveCommand">保存命令</param>
    /// <returns>物品列表</returns>
    List<InventoryItemView> GetInventoryItems(CultivateProject cultivateProject, List<Material> metadata, ICommand saveCommand);

    /// <summary>
    /// 异步获取统计物品列表
    /// </summary>
    /// <param name="cultivateProject">养成计划</param>
    /// <param name="token">取消令牌</param>
    /// <returns>统计物品列表</returns>
    Task<ObservableCollection<StatisticsCultivateItem>> GetStatisticsCultivateItemCollectionAsync(CultivateProject cultivateProject, CancellationToken token);

    /// <summary>
    /// 删除养成清单
    /// </summary>
    /// <param name="entryId">入口Id</param>
    /// <returns>任务</returns>
    Task RemoveCultivateEntryAsync(Guid entryId);

    /// <summary>
    /// 异步移除项目
    /// </summary>
    /// <param name="project">项目</param>
    /// <returns>任务</returns>
    Task RemoveProjectAsync(CultivateProject project);

    /// <summary>
    /// 异步保存养成物品
    /// </summary>
    /// <param name="type">类型</param>
    /// <param name="itemId">主Id</param>
    /// <param name="items">待存物品</param>
    /// <returns>是否保存成功</returns>
    Task<bool> SaveConsumptionAsync(CultivateType type, int itemId, List<Item> items);

    /// <summary>
    /// 保存养成物品状态
    /// </summary>
    /// <param name="item">养成物品</param>
    void SaveCultivateItem(Model.Entity.CultivateItem item);

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
    Task<ProjectAddResult> TryAddProjectAsync(CultivateProject project);
}