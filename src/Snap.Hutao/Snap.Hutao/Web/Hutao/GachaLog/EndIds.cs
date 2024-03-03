﻿// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Web.Hoyolab.Hk4e.Event.GachaInfo;

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
    public long this[GachaType type]
    {
        get
        {
            return type switch
            {
                GachaType.NewBie => NoviceWish,
                GachaType.Standard => StandardWish,
                GachaType.ActivityAvatar => AvatarEventWish,
                GachaType.ActivityWeapon => WeaponEventWish,
                _ => 0,
            };
        }

        set
        {
            switch (type)
            {
                case GachaType.NewBie:
                    NoviceWish = value;
                    break;
                case GachaType.Standard:
                    StandardWish = value;
                    break;
                case GachaType.ActivityAvatar:
                    AvatarEventWish = value;
                    break;
                case GachaType.ActivityWeapon:
                    WeaponEventWish = value;
                    break;
            }
        }
    }

    /// <summary>
    /// 获取枚举器
    /// </summary>
    /// <returns>枚举器</returns>
    public IEnumerator<KeyValuePair<GachaType, long>> GetEnumerator()
    {
        yield return new(GachaType.NewBie, NoviceWish);
        yield return new(GachaType.Standard, StandardWish);
        yield return new(GachaType.ActivityAvatar, AvatarEventWish);
        yield return new(GachaType.ActivityWeapon, WeaponEventWish);
    }
}