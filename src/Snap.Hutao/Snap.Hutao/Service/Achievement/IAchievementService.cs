// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.InterChange.Achievement;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using BindingAchievement = Snap.Hutao.Model.Binding.Achievement;
using EntityArchive = Snap.Hutao.Model.Entity.AchievementArchive;
using MetadataAchievement = Snap.Hutao.Model.Metadata.Achievement.Achievement;

namespace Snap.Hutao.Service.Achievement;

/// <summary>
/// 成就服务抽象
/// </summary>
internal interface IAchievementService
{
    /// <summary>
    /// 当前存档
    /// </summary>
    EntityArchive? CurrentArchive { get; set; }

    /// <summary>
    /// 获取整合的成就
    /// </summary>
    /// <param name="archive">用户</param>
    /// <param name="metadata">元数据</param>
    /// <returns>整合的成就</returns>
    List<BindingAchievement> GetAchievements(EntityArchive archive, IList<MetadataAchievement> metadata);

    /// <summary>
    /// 获取用于绑定的成就存档集合
    /// </summary>
    /// <returns>成就存档集合</returns>
    ObservableCollection<EntityArchive> GetArchiveCollection();

    /// <summary>
    /// 导入UIAF数据
    /// </summary>
    /// <param name="archive">用户</param>
    /// <param name="list">UIAF数据</param>
    /// <param name="option">选项</param>
    /// <returns>导入结果</returns>
    ImportResult ImportFromUIAF(EntityArchive archive, List<UIAFItem> list, ImportOption option);

    /// <summary>
    /// 异步导入UIAF数据
    /// </summary>
    /// <param name="archive">用户</param>
    /// <param name="list">UIAF数据</param>
    /// <param name="option">选项</param>
    /// <returns>导入结果</returns>
    Task<ImportResult> ImportFromUIAFAsync(EntityArchive archive, List<UIAFItem> list, ImportOption option);

    /// <summary>
    /// 异步移除存档
    /// </summary>
    /// <param name="archive">待移除的存档</param>
    /// <returns>任务</returns>
    Task RemoveArchiveAsync(EntityArchive archive);

    /// <summary>
    /// 保存成就
    /// </summary>
    /// <param name="archive">用户</param>
    /// <param name="achievements">成就</param>
    void SaveAchievements(EntityArchive archive, IList<BindingAchievement> achievements);

    /// <summary>
    /// 尝试添加存档
    /// </summary>
    /// <param name="newArchive">新存档</param>
    /// <returns>存档添加结果</returns>
    Task<ArchiveAddResult> TryAddArchiveAsync(EntityArchive newArchive);
}