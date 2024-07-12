// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Database;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Model.InterChange.GachaLog;

namespace Snap.Hutao.Service.GachaLog;

/// <summary>
/// 祈愿记录导入服务
/// </summary>
internal interface IUIGFImportService
{
    ValueTask ImportAsync(GachaLogServiceMetadataContext context, LegacyUIGF uigf, AdvancedDbCollectionView<GachaArchive> archives);
}