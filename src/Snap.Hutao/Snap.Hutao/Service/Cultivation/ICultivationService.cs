// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Binding.Cultivation;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Model.Metadata;
using Snap.Hutao.Model.Primitive;
using Snap.Hutao.Web.Hoyolab.Takumi.Event.Calculate;
using System.Collections.ObjectModel;

namespace Snap.Hutao.Service.Cultivation;

/// <summary>
/// 养成计算服务
/// </summary>
internal interface ICultivationService
{
    /// <summary>
    /// 当前养成计划
    /// </summary>
    CultivateProject? Current { get; set; }

    /// <summary>
    /// 获取绑定用的养成列表
    /// </summary>
    /// <param name="cultivateProject">养成计划</param>
    /// <param name="materials">材料</param>
    /// <param name="idAvatarMap">Id角色映射</param>
    /// <param name="idWeaponMap">Id武器映射</param>
    /// <returns>绑定用的养成列表</returns>
    Task<ObservableCollection<Model.Binding.Cultivation.CultivateEntry>> GetCultivateEntriesAsync(CultivateProject cultivateProject, List<Material> materials, Dictionary<AvatarId, Model.Metadata.Avatar.Avatar> idAvatarMap, Dictionary<WeaponId, Model.Metadata.Weapon.Weapon> idWeaponMap);

    /// <summary>
    /// 获取物品列表
    /// </summary>
    /// <param name="cultivateProject">养成计划</param>
    /// <param name="metadata">元数据</param>
    /// <returns>物品列表</returns>
    List<Model.Binding.Inventory.InventoryItem> GetInventoryItems(CultivateProject cultivateProject, List<Material> metadata);

    /// <summary>
    /// 获取用于绑定的项目集合
    /// </summary>
    /// <returns>项目集合</returns>
    ObservableCollection<CultivateProject> GetProjectCollection();

    /// <summary>
    /// 异步获取统计物品列表
    /// </summary>
    /// <param name="cultivateProject">养成计划</param>
    /// <param name="materials">元数据</param>
    /// <returns>统计物品列表</returns>
    Task<List<StatisticsCultivateItem>> GetStatisticsCultivateItemsAsync(CultivateProject cultivateProject, List<Material> materials);

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
    void SaveInventoryItem(Model.Binding.Inventory.InventoryItem item);

    /// <summary>
    /// 异步尝试添加新的项目
    /// </summary>
    /// <param name="project">项目</param>
    /// <returns>添加操作的结果</returns>
    Task<ProjectAddResult> TryAddProjectAsync(CultivateProject project);
}