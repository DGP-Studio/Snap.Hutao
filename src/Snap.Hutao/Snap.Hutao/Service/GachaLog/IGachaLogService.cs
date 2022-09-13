// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Entity;
using System.Collections.ObjectModel;

namespace Snap.Hutao.Service.GachaLog;

/// <summary>
/// 祈愿记录服务
/// </summary>
internal interface IGachaLogService
{
    /// <summary>
    /// 当前存档
    /// </summary>
    GachaArchive? CurrentArchive { get; set; }

    /// <summary>
    /// 获取可用于绑定的存档集合
    /// </summary>
    /// <returns>存档集合</returns>
    ObservableCollection<GachaArchive> GetArchiveCollection();
}
