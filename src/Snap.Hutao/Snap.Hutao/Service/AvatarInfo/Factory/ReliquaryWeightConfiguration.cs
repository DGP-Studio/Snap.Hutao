// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Intrinsic;
using Snap.Hutao.Model.Metadata;

namespace Snap.Hutao.Service.AvatarInfo.Factory;

/// <summary>
/// 权重配置
/// </summary>
internal static partial class ReliquaryWeightConfiguration
{
    /// <summary>
    /// 默认
    /// </summary>
    public static readonly AffixWeight Default = new(0, 100, 75, 0, 100, 100, 0, 55, 0) { { FightProperty.FIGHT_PROP_WATER_ADD_HURT, 100 } };

    /// <summary>
    /// 词条权重
    /// </summary>
    public static readonly List<AffixWeight> AffixWeights = new()
    {
        new(AvatarIds.Ayaka, 0, 75, 0, 100, 100, 0, 0, 0) { { FightProperty.FIGHT_PROP_ICE_ADD_HURT, 100 } },
        new(AvatarIds.Qin, 0, 75, 0, 100, 100, 0, 55, 100) { { FightProperty.FIGHT_PROP_WIND_ADD_HURT, 100 } },
        new(AvatarIds.Lisa, 0, 75, 0, 100, 100, 75, 0, 0) { { FightProperty.FIGHT_PROP_ELEC_ADD_HURT, 100 } },
        new(AvatarIds.Barbara, 100, 50, 0, 50, 50, 0, 55, 100) { { FightProperty.FIGHT_PROP_WATER_ADD_HURT, 80 } },
        new(AvatarIds.Barbara, 50, 75, 0, 100, 100, 0, 55, 100, "暴力奶妈") { { FightProperty.FIGHT_PROP_WIND_ADD_HURT, 100 } },
        new(AvatarIds.Kaeya, 0, 75, 0, 100, 100, 0, 0, 0) { { FightProperty.FIGHT_PROP_ICE_ADD_HURT, 100 } },
        new(AvatarIds.Diluc, 0, 75, 0, 100, 100, 75, 0, 0) { { FightProperty.FIGHT_PROP_FIRE_ADD_HURT, 100 } },
        new(AvatarIds.Razor, 0, 75, 0, 100, 100, 0, 0, 0) { { FightProperty.FIGHT_PROP_ELEC_ADD_HURT, 50 }, { FightProperty.FIGHT_PROP_PHYSICAL_ADD_HURT, 100 } },
        new(AvatarIds.Ambor, 0, 75, 0, 100, 100, 75, 0, 0) { { FightProperty.FIGHT_PROP_FIRE_ADD_HURT, 100 } },
        new(AvatarIds.Venti, 0, 75, 0, 100, 100, 75, 55, 0) { { FightProperty.FIGHT_PROP_WIND_ADD_HURT, 100 } },
        new(AvatarIds.Xiangling, 0, 75, 0, 100, 100, 75, 55, 0) { { FightProperty.FIGHT_PROP_FIRE_ADD_HURT, 100 } },
        new(AvatarIds.Beidou, 0, 75, 0, 100, 100, 75, 55, 0) { { FightProperty.FIGHT_PROP_ELEC_ADD_HURT, 100 } },
        new(AvatarIds.Xingqiu, 0, 75, 0, 100, 100, 0, 75, 0) { { FightProperty.FIGHT_PROP_WATER_ADD_HURT, 100 } },
        new(AvatarIds.Xiao, 0, 75, 0, 100, 100, 0, 55, 0) { { FightProperty.FIGHT_PROP_WIND_ADD_HURT, 100 } },
        new(AvatarIds.Ningguang, 0, 75, 0, 100, 100, 0, 30, 0) { { FightProperty.FIGHT_PROP_ROCK_ADD_HURT, 100 } },
        new(AvatarIds.Klee, 0, 75, 0, 100, 100, 75, 0, 0) { { FightProperty.FIGHT_PROP_FIRE_ADD_HURT, 100 } },
        new(AvatarIds.Zhongli, 80, 75, 0, 100, 100, 0, 55, 0, "武神钟离") { { FightProperty.FIGHT_PROP_ROCK_ADD_HURT, 100 }, { FightProperty.FIGHT_PROP_PHYSICAL_ADD_HURT, 50 } },
        new(AvatarIds.Zhongli, 100, 55, 0, 100, 100, 0, 55, 0, "血牛钟离") { { FightProperty.FIGHT_PROP_ROCK_ADD_HURT, 75 } },
        new(AvatarIds.Zhongli, 100, 55, 0, 100, 100, 0, 75, 0, "血牛钟离（2命+）") { { FightProperty.FIGHT_PROP_ROCK_ADD_HURT, 75 } },
        new(AvatarIds.Fischl, 0, 75, 0, 100, 100, 0, 0, 0) { { FightProperty.FIGHT_PROP_ELEC_ADD_HURT, 100 } },
        new(AvatarIds.Bennett, 100, 50, 0, 100, 100, 0, 55, 100) { { FightProperty.FIGHT_PROP_FIRE_ADD_HURT, 70 } },
        new(AvatarIds.Tartaglia, 0, 75, 0, 100, 100, 75, 0, 0) { { FightProperty.FIGHT_PROP_WATER_ADD_HURT, 100 } },
        new(AvatarIds.Noel, 0, 50, 90, 100, 100, 0, 70, 0) { { FightProperty.FIGHT_PROP_ROCK_ADD_HURT, 100 } },
        new(AvatarIds.Qiqi, 0, 100, 0, 100, 100, 0, 55, 100) { { FightProperty.FIGHT_PROP_ICE_ADD_HURT, 60 } },
        new(AvatarIds.Chongyun, 0, 75, 0, 100, 100, 75, 55, 0) { { FightProperty.FIGHT_PROP_ICE_ADD_HURT, 100 } },
        new(AvatarIds.Ganyu, 0, 75, 0, 100, 100, 75, 0, 0, "融化流") { { FightProperty.FIGHT_PROP_ICE_ADD_HURT, 100 } },
        new(AvatarIds.Ganyu, 0, 75, 0, 100, 100, 0, 55, 0, "永冻流") { { FightProperty.FIGHT_PROP_ICE_ADD_HURT, 100 } },
        new(AvatarIds.Albedo, 0, 0, 100, 100, 100, 0, 0, 0) { { FightProperty.FIGHT_PROP_ROCK_ADD_HURT, 100 } },
        new(AvatarIds.Diona, 100, 50, 0, 50, 50, 0, 90, 100) { { FightProperty.FIGHT_PROP_ICE_ADD_HURT, 100 } },
        new(AvatarIds.Mona, 0, 75, 0, 100, 100, 75, 75, 0) { { FightProperty.FIGHT_PROP_WATER_ADD_HURT, 100 } },
        new(AvatarIds.Keqing, 0, 75, 0, 100, 100, 0, 0, 0) { { FightProperty.FIGHT_PROP_ELEC_ADD_HURT, 100 }, { FightProperty.FIGHT_PROP_PHYSICAL_ADD_HURT, 100 } },
        new(AvatarIds.Sucrose, 0, 75, 0, 100, 100, 100, 55, 0) { { FightProperty.FIGHT_PROP_WIND_ADD_HURT, 40 } },
        new(AvatarIds.Xinyan, 0, 75, 0, 100, 100, 0, 0, 0) { { FightProperty.FIGHT_PROP_FIRE_ADD_HURT, 50 } },
        new(AvatarIds.Rosaria, 0, 75, 0, 100, 100, 0, 0, 0) { { FightProperty.FIGHT_PROP_ICE_ADD_HURT, 70 }, { FightProperty.FIGHT_PROP_PHYSICAL_ADD_HURT, 80 } },
        new(AvatarIds.Hutao, 80, 50, 0, 100, 100, 75, 0, 0) { { FightProperty.FIGHT_PROP_FIRE_ADD_HURT, 100 } },
        new(AvatarIds.Kazuha, 0, 75, 0, 100, 100, 100, 55, 0) { { FightProperty.FIGHT_PROP_WIND_ADD_HURT, 100 } },
        new(AvatarIds.Feiyan, 0, 75, 0, 100, 100, 75, 0, 0) { { FightProperty.FIGHT_PROP_FIRE_ADD_HURT, 100 } },
        new(AvatarIds.Yoimiya, 0, 75, 0, 100, 100, 75, 0, 0) { { FightProperty.FIGHT_PROP_FIRE_ADD_HURT, 100 } },
        new(AvatarIds.Tohma, 100, 50, 0, 50, 50, 0, 90, 0) { { FightProperty.FIGHT_PROP_FIRE_ADD_HURT, 75 } },
        new(AvatarIds.Eula, 0, 75, 0, 100, 100, 0, 55, 0) { { FightProperty.FIGHT_PROP_ICE_ADD_HURT, 40 }, { FightProperty.FIGHT_PROP_PHYSICAL_ADD_HURT, 100 } },
        new(AvatarIds.Shougun, 0, 75, 0, 100, 100, 0, 90, 0) { { FightProperty.FIGHT_PROP_ELEC_ADD_HURT, 75 } },
        new(AvatarIds.Sayu, 0, 50, 0, 50, 50, 100, 55, 100) { { FightProperty.FIGHT_PROP_WIND_ADD_HURT, 80 } },
        new(AvatarIds.Kokomi, 100, 50, 0, 0, 0, 0, 55, 100) { { FightProperty.FIGHT_PROP_WATER_ADD_HURT, 100 } },
        new(AvatarIds.Gorou, 0, 50, 100, 50, 50, 0, 90, 0) { { FightProperty.FIGHT_PROP_ROCK_ADD_HURT, 25 } },
        new(AvatarIds.Sara, 0, 75, 0, 100, 100, 0, 55, 0) { { FightProperty.FIGHT_PROP_ELEC_ADD_HURT, 100 } },
        new(AvatarIds.Itto, 0, 50, 100, 100, 100, 0, 30, 0) { { FightProperty.FIGHT_PROP_ROCK_ADD_HURT, 100 } },
        new(AvatarIds.Yae, 0, 75, 0, 100, 100, 75, 55, 0) { { FightProperty.FIGHT_PROP_ELEC_ADD_HURT, 100 } },
        new(AvatarIds.Heizou, 0, 75, 0, 100, 100, 75, 0, 0) { { FightProperty.FIGHT_PROP_WIND_ADD_HURT, 100 } },
        new(AvatarIds.Yelan, 80, 0, 0, 100, 100, 0, 75, 0) { { FightProperty.FIGHT_PROP_WATER_ADD_HURT, 100 } },
        new(AvatarIds.Aloy, 0, 75, 0, 100, 100, 0, 0, 0) { { FightProperty.FIGHT_PROP_ICE_ADD_HURT, 100 } },
        new(AvatarIds.Shenhe, 0, 100, 0, 100, 100, 0, 55, 0) { { FightProperty.FIGHT_PROP_ICE_ADD_HURT, 100 } },
        new(AvatarIds.Yunjin, 0, 0, 100, 50, 50, 0, 90, 0) { { FightProperty.FIGHT_PROP_ROCK_ADD_HURT, 25 } },
        new(AvatarIds.Shinobu, 100, 50, 0, 100, 100, 75, 55, 100) { { FightProperty.FIGHT_PROP_ELEC_ADD_HURT, 100 } },
        new(AvatarIds.Ayato, 50, 75, 0, 100, 100, 0, 0, 0) { { FightProperty.FIGHT_PROP_WATER_ADD_HURT, 100 } },
        new(AvatarIds.Collei, 0, 75, 0, 100, 100, 0, 55, 0) { { FightProperty.FIGHT_PROP_GRASS_ADD_HURT, 100 } },
        new(AvatarIds.Dori, 100, 75, 0, 100, 100, 0, 55, 0) { { FightProperty.FIGHT_PROP_ELEC_ADD_HURT, 100 } },
        new(AvatarIds.Tighnari, 0, 75, 0, 100, 100, 75, 55, 0) { { FightProperty.FIGHT_PROP_GRASS_ADD_HURT, 100 } },
        new(AvatarIds.Nilou, 100, 75, 0, 100, 100, 0, 55, 0, "直伤流") { { FightProperty.FIGHT_PROP_WATER_ADD_HURT, 100 } },
        new(AvatarIds.Nilou, 100, 75, 0, 100, 100, 0, 55, 0, "反应流") { { FightProperty.FIGHT_PROP_WATER_ADD_HURT, 100 } },
        new(AvatarIds.Cyno, 0, 75, 0, 100, 100, 75, 55, 0) { { FightProperty.FIGHT_PROP_ELEC_ADD_HURT, 100 } },
        new(AvatarIds.Candace, 100, 75, 0, 100, 100, 0, 55, 0) { { FightProperty.FIGHT_PROP_WATER_ADD_HURT, 100 } },
        new(AvatarIds.Nahida, 0, 55, 0, 100, 100, 100, 55, 0) { { FightProperty.FIGHT_PROP_GRASS_ADD_HURT, 100 } },
    };
}