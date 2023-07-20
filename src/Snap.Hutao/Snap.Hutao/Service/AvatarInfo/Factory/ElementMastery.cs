// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Service.AvatarInfo.Factory;

/// <summary>
/// 元素精通
/// </summary>
[HighQuality]
internal static class ElementMastery
{
    /// <summary>
    /// 增幅反应
    /// </summary>
    public static readonly ElementMasteryCoefficient ElementAddHurt = new(2.78F, 1400F);

    /// <summary>
    /// 剧变反应
    /// </summary>
    public static readonly ElementMasteryCoefficient ReactionAddHurt = new(16F, 2000F);

    /// <summary>
    /// 激化反应
    /// </summary>
    public static readonly ElementMasteryCoefficient ReactionOverdoseAddHurt = new(5F, 1200F);

    /// <summary>
    /// 结晶盾厚度
    /// </summary>
    public static readonly ElementMasteryCoefficient CrystalShieldHp = new(4.44F, 1400F);

    /// <summary>
    /// 获取差异
    /// </summary>
    /// <param name="mastery">元素精通</param>
    /// <param name="coeff">参数</param>
    /// <returns>差异</returns>
    public static float GetDelta(float mastery, in ElementMasteryCoefficient coeff)
    {
        return mastery + coeff.Param2 == 0 ? 0 : MathF.Max(mastery * coeff.Param1 / (mastery + coeff.Param2), 0);
    }
}