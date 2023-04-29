// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.InterChange.Achievement;
using Snap.Hutao.Model.Primitive;
using System.Collections.ObjectModel;
using BindingAchievement = Snap.Hutao.ViewModel.Achievement.AchievementView;
using EntityArchive = Snap.Hutao.Model.Entity.AchievementArchive;
using MetadataAchievement = Snap.Hutao.Model.Metadata.Achievement.Achievement;

namespace Snap.Hutao.Service.Achievement;

/// <summary>
/// 成就服务抽象
/// </summary>
[HighQuality]
internal interface IAchievementService
{
    /// <summary>
    /// 当前存档
    /// </summary>
    EntityArchive? CurrentArchive { get; set; }

    /// <summary>
    /// 获取用于绑定的成就存档集合
    /// </summary>
    ObservableCollection<EntityArchive> ArchiveCollection { get; }

    /// <summary>
    /// 异步导出到UIAF
    /// </summary>
    /// <param name="selectedArchive">存档</param>
    /// <returns>UIAF</returns>
    Task<UIAF> ExportToUIAFAsync(EntityArchive selectedArchive);

    /// <summary>
    /// 获取整合的成就
    /// </summary>
    /// <param name="archive">用户</param>
    /// <param name="metadata">元数据</param>
    /// <returns>整合的成就</returns>
    List<BindingAchievement> GetAchievements(EntityArchive archive, List<MetadataAchievement> metadata);

    /// <summary>
    /// 异步获取成就统计列表
    /// </summary>
    /// <param name="achievementMap">成就映射</param>
    /// <returns>成就统计列表</returns>
    Task<List<ViewModel.Achievement.AchievementStatistics>> GetAchievementStatisticsAsync(Dictionary<AchievementId, MetadataAchievement> achievementMap);

    /// <summary>
    /// 异步导入UIAF数据
    /// </summary>
    /// <param name="archive">用户</param>
    /// <param name="list">UIAF数据</param>
    /// <param name="strategy">选项</param>
    /// <returns>导入结果</returns>
    Task<ImportResult> ImportFromUIAFAsync(EntityArchive archive, List<UIAFItem> list, ImportStrategy strategy);

    /// <summary>
    /// 异步移除存档
    /// </summary>
    /// <param name="archive">待移除的存档</param>
    /// <returns>任务</returns>
    Task RemoveArchiveAsync(EntityArchive archive);

    /// <summary>
    /// 保存单个成就
    /// </summary>
    /// <param name="achievement">成就</param>
    void SaveAchievement(BindingAchievement achievement);

    /// <summary>
    /// 保存成就
    /// </summary>
    /// <param name="archive">用户</param>
    /// <param name="achievements">成就</param>
    void SaveAchievements(EntityArchive archive, List<BindingAchievement> achievements);

    /// <summary>
    /// 尝试添加存档
    /// </summary>
    /// <param name="newArchive">新存档</param>
    /// <returns>存档添加结果</returns>
    Task<ArchiveAddResult> TryAddArchiveAsync(EntityArchive newArchive);
}