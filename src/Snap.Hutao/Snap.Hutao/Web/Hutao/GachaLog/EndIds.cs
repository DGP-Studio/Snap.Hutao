// Copyright (c) DGP Studio. All rights reserved.
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
}