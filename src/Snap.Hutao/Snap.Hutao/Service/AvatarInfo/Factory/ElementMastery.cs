// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Extension;
using Snap.Hutao.Model;
using Snap.Hutao.Model.Binding.AvatarProperty;
using Snap.Hutao.Model.Intrinsic;
using Snap.Hutao.Model.Metadata.Annotation;
using Snap.Hutao.Model.Metadata.Avatar;
using Snap.Hutao.Model.Metadata.Converter;
using ModelPlayerInfo = Snap.Hutao.Web.Enka.Model.PlayerInfo;

namespace Snap.Hutao.Service.AvatarInfo.Factory;

/// <summary>
/// 元素精通
/// </summary>
internal static class ElementMastery
{
    /// <summary>
    /// 增幅反应
    /// </summary>
    public static readonly ElementMasteryCoefficient ElementAddHurt = new(2.78f, 1400);

    /// <summary>
    /// 剧变反应
    /// </summary>
    public static readonly ElementMasteryCoefficient ReactionAddHurt = new(16, 2000);

    /// <summary>
    /// 激化反应
    /// </summary>
    public static readonly ElementMasteryCoefficient ReactionOverdoseAddHurt = new(5, 1200);

    /// <summary>
    /// 激化反应
    /// </summary>
    public static readonly ElementMasteryCoefficient CrystalShieldHp = new(4.44f, 1400);

    /// <summary>
    /// 获取差异
    /// </summary>
    /// <param name="mastery">元素精通</param>
    /// <param name="coeff">参数</param>
    /// <returns>差异</returns>
    public static float GetDelta(float mastery, ElementMasteryCoefficient coeff)
    {
        return mastery + coeff.P2 == 0 ? 0 : MathF.Max(mastery * coeff.P1 / (mastery + coeff.P2), 0);
    }
}