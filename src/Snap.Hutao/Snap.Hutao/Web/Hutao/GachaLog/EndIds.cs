// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.EntityFrameworkCore;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Model.Entity.Database;
using Snap.Hutao.Web.Hoyolab.Hk4e.Event.GachaInfo;
using System.Collections;

namespace Snap.Hutao.Web.Hutao.GachaLog;

/// <summary>
/// 末尾Id 字典
/// </summary>
internal sealed class EndIds
{
    /// <summary>
    /// 新手祈愿
    /// </summary>
    [JsonPropertyName("100")]
    public long NoviceWish { get; set; }

    /// <summary>
    /// 常驻祈愿
    /// </summary>
    [JsonPropertyName("200")]
    public long StandardWish { get; set; }

    /// <summary>
    /// 角色活动祈愿
    /// </summary>
    [JsonPropertyName("301")]
    public long AvatarEventWish { get; set; }

    /// <summary>
    /// 武器活动祈愿
    /// </summary>
    [JsonPropertyName("302")]
    public long WeaponEventWish { get; set; }

    /// <summary>
    /// 获取 Last Id
    /// </summary>
    /// <param name="type">类型</param>
    /// <returns>Last Id</returns>
    public long this[GachaConfigType type]
    {
        get
        {
            return type switch
            {
                GachaConfigType.NoviceWish => NoviceWish,
                GachaConfigType.StandardWish => StandardWish,
                GachaConfigType.AvatarEventWish => AvatarEventWish,
                GachaConfigType.WeaponEventWish => WeaponEventWish,
                _ => 0,
            };
        }

        set
        {
            switch (type)
            {
                case GachaConfigType.NoviceWish:
                    NoviceWish = value;
                    break;
                case GachaConfigType.StandardWish:
                    StandardWish = value;
                    break;
                case GachaConfigType.AvatarEventWish:
                    AvatarEventWish = value;
                    break;
                case GachaConfigType.WeaponEventWish:
                    WeaponEventWish = value;
                    break;
            }
        }
    }

    /// <summary>
    /// 异步创建一个新的 End Id集合
    /// </summary>
    /// <param name="appDbContext">数据库上下文</param>
    /// <param name="archive">存档</param>
    /// <param name="token">取消令牌</param>
    /// <returns>新的 End Id集合</returns>
    public static async Task<EndIds> CreateAsync(AppDbContext appDbContext, GachaArchive? archive, CancellationToken token)
    {
        EndIds endIds = new();
        foreach (GachaConfigType type in Service.GachaLog.GachaLog.QueryTypes)
        {
            if (archive != null)
            {
                Snap.Hutao.Model.Entity.GachaItem? item = await appDbContext.GachaItems
                    .Where(i => i.ArchiveId == archive.InnerId)
                    .Where(i => i.QueryType == type)
                    .OrderBy(i => i.Id)
                    .FirstOrDefaultAsync(token)
                    .ConfigureAwait(false);

                if (item != null)
                {
                    endIds[type] = item.Id;
                }
            }
        }

        return endIds;
    }

    /// <summary>
    /// 获取枚举器
    /// </summary>
    /// <returns>枚举器</returns>
    public IEnumerator<KeyValuePair<GachaConfigType, long>> GetEnumerator()
    {
        yield return new(GachaConfigType.NoviceWish, NoviceWish);
        yield return new(GachaConfigType.StandardWish, StandardWish);
        yield return new(GachaConfigType.AvatarEventWish, AvatarEventWish);
        yield return new(GachaConfigType.WeaponEventWish, WeaponEventWish);
    }
}