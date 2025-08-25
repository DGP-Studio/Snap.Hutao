// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Model.Intrinsic;

[ExtendedEnum]
internal enum ElementType
{
    None = 0, // 无元素

    [LocalizationKey(nameof(SH.ModelIntrinsicElementNameFire))]
    Fire = 1, // 火元素

    [LocalizationKey(nameof(SH.ModelIntrinsicElementNameWater))]
    Water = 2, // 水元素

    [LocalizationKey(nameof(SH.ModelIntrinsicElementNameGrass))]
    Grass = 3, // 草元素

    [LocalizationKey(nameof(SH.ModelIntrinsicElementNameElec))]
    Electric = 4, // 雷元素

    [LocalizationKey(nameof(SH.ModelIntrinsicElementNameIce))]
    Ice = 5, // 冰元素
    Frozen = 6, // 冻元素

    [LocalizationKey(nameof(SH.ModelIntrinsicElementNameWind))]
    Wind = 7, // 风元素

    [LocalizationKey(nameof(SH.ModelIntrinsicElementNameRock))]
    Rock = 8, // 岩元素
    AntiFire = 9, // 燃元素
    VehicleMuteIce = 10, // ?
    Mushroom = 11, // 弹弹菇
    Overdose = 12, // 激元素
    Wood = 13, // 木元素
    LiquidPhlogiston = 14, // 液态燃素
    SolidPhlogiston = 15, // ?
    SolidifyPhlogiston = 16, // ?
    Count, // 个数
}