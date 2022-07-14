// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Data;
using Snap.Hutao.Model.Intrinsic;

namespace Snap.Hutao.Model.Metadata.Converter;

/// <summary>
/// 元素名称图标转换器
/// </summary>
internal class WeaponTypeIconConverter : IValueConverter
{
    private const string BaseUrl = "https://static.snapgenshin.com/Skill/Skill_A_{0}.png";

    /// <inheritdoc/>
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        string element = (WeaponType)value switch
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
    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw Must.NeverHappen();
    }
}