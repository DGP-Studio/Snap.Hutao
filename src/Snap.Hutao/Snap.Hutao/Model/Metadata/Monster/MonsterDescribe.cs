// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Primitive;

namespace Snap.Hutao.Model.Metadata.Monster;

internal static class MonsterDescribe
{
    public static MonsterDescribeId Normalize(in MonsterDescribeId id)
    {
        return (uint)id switch
        {
            5020U => 502U,    // 幻形豕兽 · 水
            5021U => 502U,    // 幻形豕兽 · 水 (强化)
            5040U => 504U,    // 幻形蟹 · 水
            5041U => 504U,    // 幻形蟹 · 水 (强化)
            5070U => 507U,    // 幻形花鼠 · 水
            5071U => 507U,    // 幻形花鼠 · 水 (强化)
            5102U => 510U,    // 历经百战的浊水粉碎幻灵
            5112U => 511U,    // 历经百战的浊水喷吐幻灵
            30121U => 30111U, // 历经百战的愚人众先遣队·冰铳重卫士
            30123U => 30113U, // 历经百战的愚人众先遣队·雷锤前锋军
            30126U => 30116U, // 历经百战的愚人众先遣队·火铳游击兵
            30605U => 30603U, // 历经百战的霜剑律从
            30606U => 30604U, // 历经百战的幽风铃兰
            40632U => 40613U, // 自律超算型场力发生装置
            60402U => 60401U, // (火)岩龙蜥
            60403U => 60401U, // (冰)岩龙蜥
            60404U => 60401U, // (雷)岩龙蜥
            28021302U => 80213U, // 历经百战的玳龟
            _ => id,
        };
    }
}