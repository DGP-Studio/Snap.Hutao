// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.Hk4e.Event.GachaInfo;

internal static class GachaConfigTypeExtension
{
    /// <summary>
    /// 将祈愿配置类型转换到祈愿查询类型
    /// </summary>
    /// <param name="configType">配置类型</param>
    /// <returns>祈愿查询类型</returns>
    public static GachaConfigType ToQueryType(this GachaConfigType configType)
    {
        return configType switch
        {
            GachaConfigType.AvatarEventWish2 => GachaConfigType.AvatarEventWish,
            _ => configType,
        };
    }
}