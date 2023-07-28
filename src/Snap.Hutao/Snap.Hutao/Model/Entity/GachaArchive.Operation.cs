// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Snap.Hutao.Core.Database;
using Snap.Hutao.Core.ExceptionService;
using Snap.Hutao.Service.GachaLog;
using Snap.Hutao.Web.Hoyolab.Hk4e.Event.GachaInfo;

namespace Snap.Hutao.Model.Entity;

/// <summary>
/// 操作部分
/// </summary>
internal sealed partial class GachaArchive
{
    /// <summary>
    /// 保存祈愿物品
    /// </summary>
    /// <param name="context">上下文</param>
    [SuppressMessage("", "SH002")]
    public void SaveItems(GachaItemSaveContext context)
    {
        if (context.ItemsToAdd.Count > 0)
        {
            // 全量刷新
            if (!context.IsLazy)
            {
                context.GachaItems
                    .Where(i => i.ArchiveId == InnerId)
                    .Where(i => i.Id >= context.EndId)
                    .ExecuteDelete();
            }

            context.GachaItems.AddRangeAndSave(context.ItemsToAdd);
        }
    }
}
