// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Snap.Hutao.Core.Database;
using Snap.Hutao.Core.Diagnostics;
using Snap.Hutao.Core.ExceptionService;
using Snap.Hutao.Model.Binding;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Model.Entity.Database;
using Snap.Hutao.Model.InterChange.GachaLog;
using Snap.Hutao.Model.Metadata.Abstraction;
using Snap.Hutao.Model.Primitive;
using Snap.Hutao.Service.GachaLog.Factory;
using Snap.Hutao.Service.GachaLog.QueryProvider;
using Snap.Hutao.Service.Metadata;
using Snap.Hutao.ViewModel.GachaLog;
using Snap.Hutao.Web.Hoyolab.Hk4e.Event.GachaInfo;
using Snap.Hutao.Web.Response;
using System.Collections.Immutable;
using System.Collections.ObjectModel;

namespace Snap.Hutao.Service.GachaLog;

/// <summary>
/// 祈愿记录导入服务
/// </summary>
[Injection(InjectAs.Scoped, typeof(IGachaLogImportService))]
internal sealed class GachaLogImportService : IGachaLogImportService
{
    private readonly AppDbContext appDbContext;
    private readonly ILogger<GachaLogImportService> logger;

    /// <summary>
    /// 构造一个新的祈愿记录导入服务
    /// </summary>
    /// <param name="appDbContext">数据库上下文</param>
    /// <param name="logger">日志器</param>
    public GachaLogImportService(AppDbContext appDbContext, ILogger<GachaLogImportService> logger)
    {
        this.appDbContext = appDbContext;
        this.logger = logger;
    }

    /// <inheritdoc/>
    public async Task<GachaArchive> ImportFromUIGFAsync(GachaLogServiceContext context, List<UIGFItem> list, string uid)
    {
        GachaArchive.Init(out GachaArchive? archive, uid, appDbContext.GachaArchives, context.ArchiveCollection);
        await ThreadHelper.SwitchToBackgroundAsync();
        Guid archiveId = archive.InnerId;

        long trimId = appDbContext.GachaItems
            .Where(i => i.ArchiveId == archiveId)
            .OrderBy(i => i.Id)
            .FirstOrDefault()?.Id ?? long.MaxValue;

        logger.LogInformation("Last Id to trim with [{id}]", trimId);

        IEnumerable<GachaItem> toAdd = list
            .OrderByDescending(i => i.Id)
            .Where(i => i.Id < trimId)
            .Select(i => GachaItem.Create(archiveId, i, GetItemId(context, i)));

        await appDbContext.GachaItems.AddRangeAndSaveAsync(toAdd).ConfigureAwait(false);
        return archive;
    }

    private static int GetItemId(GachaLogServiceContext context, GachaLogItem item)
    {
        return item.ItemType switch
        {
            "角色" => context.NameAvatarMap.GetValueOrDefault(item.Name)?.Id ?? 0,
            "武器" => context.NameWeaponMap.GetValueOrDefault(item.Name)?.Id ?? 0,
            _ => 0,
        };
    }
}