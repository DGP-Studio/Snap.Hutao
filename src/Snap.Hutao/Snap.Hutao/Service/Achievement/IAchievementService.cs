// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.InterChange.Achievement;
using Snap.Hutao.ViewModel.Achievement;
using System.Collections.ObjectModel;
using EntityArchive = Snap.Hutao.Model.Entity.AchievementArchive;

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
    ValueTask<UIAF> ExportToUIAFAsync(EntityArchive selectedArchive);

    List<AchievementView> GetAchievementViewList(EntityArchive archive, AchievementServiceMetadataContext context);

    /// <summary>
    /// 异步导入UIAF数据
    /// </summary>
    /// <param name="archive">用户</param>
    /// <param name="list">UIAF数据</param>
    /// <param name="strategy">选项</param>
    /// <returns>导入结果</returns>
    ValueTask<ImportResult> ImportFromUIAFAsync(EntityArchive archive, List<UIAFItem> list, ImportStrategyKind strategy);

    /// <summary>
    /// 异步移除存档
    /// </summary>
    /// <param name="archive">待移除的存档</param>
    /// <returns>任务</returns>
    ValueTask RemoveArchiveAsync(EntityArchive archive);

    /// <summary>
    /// 保存单个成就
    /// </summary>
    /// <param name="achievement">成就</param>
    void SaveAchievement(AchievementView achievement);

    /// <summary>
    /// 尝试添加存档
    /// </summary>
    /// <param name="newArchive">新存档</param>
    /// <returns>存档添加结果</returns>
    ValueTask<ArchiveAddResultKind> AddArchiveAsync(EntityArchive newArchive);
}