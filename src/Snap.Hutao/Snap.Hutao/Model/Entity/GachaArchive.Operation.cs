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
    /// 初始化或跳过
    /// </summary>
    /// <param name="context">上下文</param>
    /// <param name="archive">存档</param>
    public static void SkipOrInit(in GachaArchiveInitializationContext context, [NotNull] ref GachaArchive? archive)
    {
        if (archive == null)
        {
            Init(context, out archive);
        }
    }

    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="context">上下文</param>
    /// <param name="archive">存档</param>
    [SuppressMessage("", "SH002")]
    public static void Init(GachaArchiveInitializationContext context, [NotNull] out GachaArchive? archive)
    {
        archive = context.ArchiveCollection.SingleOrDefault(a => a.Uid == context.Uid);

        if (archive == null)
        {
            GachaArchive created = From(context.Uid);
            context.GachaArchives.AddAndSave(created);
            context.TaskContext.InvokeOnMainThread(() => context.ArchiveCollection.Add(created));
            archive = created;
        }
    }

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

    /// <summary>
    /// 按卡池类型获取数据库中的最大 Id
    /// </summary>
    /// <param name="configType">卡池类型</param>
    /// <param name="gachaItems">数据集</param>
    /// <returns>最大 Id</returns>
    public long GetEndId(GachaConfigType configType, DbSet<GachaItem> gachaItems)
    {
        GachaItem? item = null;

        try
        {
            // TODO: replace with MaxBy
            // https://github.com/dotnet/efcore/issues/25566
            // .MaxBy(i => i.Id);
            item = gachaItems
                .Where(i => i.ArchiveId == InnerId)
                .Where(i => i.QueryType == configType)
                .OrderByDescending(i => i.Id)
                .FirstOrDefault();
        }
        catch (SqliteException ex)
        {
            ThrowHelper.UserdataCorrupted(SH.ServiceGachaLogEndIdUserdataCorruptedMessage, ex);
        }

        return item?.Id ?? 0L;
    }
}
