// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Entity;
using Snap.Hutao.ViewModel.User;
using System.Collections.ObjectModel;

namespace Snap.Hutao.Service.DailyNote;

/// <summary>
/// 实时便笺服务
/// </summary>
[HighQuality]
internal interface IDailyNoteService
{
    /// <summary>
    /// 添加实时便笺
    /// </summary>
    /// <param name="role">角色</param>
    /// <returns>任务</returns>
    Task AddDailyNoteAsync(UserAndUid role);

    /// <summary>
    /// 异步获取实时便笺列表
    /// </summary>
    /// <returns>实时便笺列表</returns>
    Task<ObservableCollection<DailyNoteEntry>> GetDailyNoteEntriesAsync();

    /// <summary>
    /// 异步刷新实时便笺
    /// </summary>
    /// <returns>任务</returns>
    ValueTask RefreshDailyNotesAsync();

    /// <summary>
    /// 移除指定的实时便笺
    /// </summary>
    /// <param name="entry">指定的实时便笺</param>
    /// <returns>任务</returns>
    Task RemoveDailyNoteAsync(DailyNoteEntry entry);
}