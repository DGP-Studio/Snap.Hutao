// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Control;
using Snap.Hutao.Model.Intrinsic;

namespace Snap.Hutao.Model.Metadata.Converter;

/// <summary>
/// 元素名称图标转换器
/// </summary>
[HighQuality]
internal sealed class WeaponTypeIconConverter : ValueConverter<WeaponType, Uri>
{
    /// <summary>
    /// 将武器类型转换为图标链接
    /// </summary>
    /// <param name="type">武器类型</param>
    /// <returns>图标链接</returns>
    public static Uri WeaponTypeToIconUri(WeaponType type)
    {
        string element = type switch
        {
            WeaponType.WEAPON_SWORD_ONE_HAND => "01",
            WeaponType.WEAPON_BOW => "02",
            WeaponType.WEAPON_POLE => "03",
            WeaponType.WEAPON_CLAYMORE => "04",
            WeaponType.WEAPON_CATALYST => "Catalyst_MD",
            _ => throw Must.NeverHappen(),
        };

        return Web.HutaoEndpoints.StaticFile("Skill", $"Skill_A_{element}.png").ToUri();
    }

    /// <inheritdoc/>
    public override Uri Convert(WeaponType from)
    {
        return WeaponTypeToIconUri(from);
    }
}