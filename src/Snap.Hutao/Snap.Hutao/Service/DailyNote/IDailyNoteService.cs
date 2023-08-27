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
    /// <param name="userAndUid">角色</param>
    /// <returns>任务</returns>
    ValueTask AddDailyNoteAsync(UserAndUid userAndUid);

    ValueTask<ObservableCollection<DailyNoteEntry>> GetDailyNoteEntryCollectionAsync(bool forceRefresh = false);

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
    ValueTask RemoveDailyNoteAsync(DailyNoteEntry entry);

    ValueTask UpdateDailyNoteAsync(DailyNoteEntry entry);
}