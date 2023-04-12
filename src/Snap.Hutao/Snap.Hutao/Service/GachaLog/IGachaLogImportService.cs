// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Entity;
using Snap.Hutao.Model.InterChange.GachaLog;

namespace Snap.Hutao.Service.GachaLog;

/// <summary>
/// 祈愿记录导入服务
/// </summary>
internal interface IGachaLogImportService
{
    /// <summary>
    /// 异步从 UIGF 导入
    /// </summary>
    /// <param name="context">祈愿记录服务上下文</param>
    /// <param name="list">列表</param>
    /// <param name="uid">uid</param>
    /// <returns>存档</returns>
    Task<GachaArchive> ImportFromUIGFAsync(GachaLogServiceContext context, List<UIGFItem> list, string uid);
}