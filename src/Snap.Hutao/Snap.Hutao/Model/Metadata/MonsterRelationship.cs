// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Primitive;

namespace Snap.Hutao.Model.Metadata;

internal static class MonsterRelationship
{
    public static MonsterRelationshipId Normalize(in MonsterRelationshipId id)
    {
        return (uint)id switch
        {
            5020U => 502U,      // 幻形豕兽 · 水
            5021U => 502U,      // 幻形豕兽 · 水 (强化)
            5040U => 504U,      // 幻形蟹 · 水
            5041U => 504U,      // 幻形蟹 · 水 (强化)
            5070U => 507U,      // 幻形花鼠 · 水
            5071U => 507U,      // 幻形花鼠 · 水 (强化)
            5102U => 510U,      // 历经百战的浊水粉碎幻灵
            5112U => 511U,      // 历经百战的浊水喷吐幻灵
            30605U => 30603U,   // 历经百战的霜剑律从
            30606U => 30604U,   // 历经百战的幽风铃兰
            60402U => 60401U,   // (火)岩龙蜥
            60403U => 60401U,   // (冰)岩龙蜥
            60404U => 60401U,   // (雷)岩龙蜥
            _ => id,
        };
    }
}