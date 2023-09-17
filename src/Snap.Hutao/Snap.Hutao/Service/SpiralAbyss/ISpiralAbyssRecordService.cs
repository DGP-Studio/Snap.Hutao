// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.ViewModel.SpiralAbyss;
using Snap.Hutao.ViewModel.User;
using System.Collections.ObjectModel;

namespace Snap.Hutao.Service.SpiralAbyss;

/// <summary>
/// 深渊记录服务
/// </summary>
[HighQuality]
internal interface ISpiralAbyssRecordService
{
    /// <summary>
    /// 异步获取深渊记录集合
    /// </summary>
    /// <param name="userAndUid">当前角色</param>
    /// <returns>深渊记录集合</returns>
    ValueTask<ObservableCollection<SpiralAbyssView>> GetSpiralAbyssViewCollectionAsync(UserAndUid userAndUid);

    ValueTask<bool> InitializeAsync();

    /// <summary>
    /// 异步刷新深渊记录
    /// </summary>
    /// <param name="userAndUid">当前角色</param>
    /// <returns>任务</returns>
    ValueTask RefreshSpiralAbyssAsync(UserAndUid userAndUid);
}