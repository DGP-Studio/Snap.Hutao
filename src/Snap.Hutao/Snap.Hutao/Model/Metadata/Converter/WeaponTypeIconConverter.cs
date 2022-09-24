// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Control;
using Snap.Hutao.Model.Intrinsic;

namespace Snap.Hutao.Model.Metadata.Converter;

/// <summary>
/// 元素名称图标转换器
/// </summary>
internal class WeaponTypeIconConverter : ValueConverterBase<WeaponType, Uri>
{
    private const string BaseUrl = "https://static.snapgenshin.com/Skill/Skill_A_{0}.png";

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

        return new Uri(string.Format(BaseUrl, element));
    }

    /// <inheritdoc/>
    public override Uri Convert(WeaponType from)
    {
        return WeaponTypeToIconUri(from);
    }
}