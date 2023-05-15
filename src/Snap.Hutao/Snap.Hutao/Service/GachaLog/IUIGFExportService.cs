// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Entity;
using Snap.Hutao.Model.InterChange.GachaLog;

namespace Snap.Hutao.Service.GachaLog;

/// <summary>
/// 祈愿记录导出服务
/// </summary>
internal interface IUIGFExportService
{
    /// <summary>
    /// 异步导出存档到 UIGF
    /// </summary>
    /// <param name="context">元数据上下文</param>
    /// <param name="archive">存档</param>
    /// <returns>UIGF</returns>
    Task<UIGF> ExportAsync(GachaLogServiceContext context, GachaArchive archive);
}
