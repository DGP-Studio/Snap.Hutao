// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Snap.Hutao.Core.Database;
using Snap.Hutao.Core.ExceptionService;
using Snap.Hutao.Web.Hoyolab.Hk4e.Event.GachaInfo;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Snap.Hutao.Model.Entity;

/// <summary>
/// 祈愿记录存档
/// </summary>
[HighQuality]
[Table("gacha_archives")]
internal sealed class GachaArchive : ISelectable
{
    /// <summary>
    /// 内部Id
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid InnerId { get; set; }

    /// <summary>
    /// 记录的Uid
    /// </summary>
    public string Uid { get; set; } = default!;

    /// <inheritdoc/>
    public bool IsSelected { get; set; }

    /// <summary>
    /// 构造一个新的卡池存档
    /// </summary>
    /// <param name="uid">uid</param>
    /// <returns>新的卡池存档</returns>
    public static GachaArchive Create(string uid)
    {
        return new() { Uid = uid };
    }

    /// <summary>
    /// 初始化或跳过
    /// </summary>
    /// <param name="archive">存档</param>
    /// <param name="uid">uid</param>
    /// <param name="gachaArchives">数据库集</param>
    /// <param name="collection">集合</param>
    public static void SkipOrInit([NotNull] ref GachaArchive? archive, string uid, DbSet<GachaArchive> gachaArchives, ObservableCollection<GachaArchive> collection)
    {
        if (archive == null)
        {
            Init(out archive, uid, gachaArchives, collection);
        }
    }

    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="archive">存档</param>
    /// <param name="uid">uid</param>
    /// <param name="gachaArchives">数据库集</param>
    /// <param name="collection">集合</param>
    public static void Init([NotNull] out GachaArchive? archive, string uid, DbSet<GachaArchive> gachaArchives, ObservableCollection<GachaArchive> collection)
    {
        archive = collection.SingleOrDefault(a => a.Uid == uid);

        if (archive == null)
        {
            GachaArchive created = Create(uid);
            gachaArchives.AddAndSave(created);
            ThreadHelper.InvokeOnMainThread(() => collection!.Add(created));
            archive = created;
        }
    }

    /// <summary>
    /// 保存祈愿物品
    /// </summary>
    /// <param name="itemsToAdd">待添加物品</param>
    /// <param name="isLazy">是否懒惰</param>
    /// <param name="endId">结尾Id</param>
    /// <param name="gachaItems">数据集</param>
    public void SaveItems(List<GachaItem> itemsToAdd, bool isLazy, long endId, DbSet<GachaItem> gachaItems)
    {
        if (itemsToAdd.Count > 0)
        {
            // 全量刷新
            if (!isLazy)
            {
                gachaItems
                    .Where(i => i.ArchiveId == InnerId)
                    .Where(i => i.Id >= endId)
                    .ExecuteDelete();
            }

            gachaItems.AddRangeAndSave(itemsToAdd);
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