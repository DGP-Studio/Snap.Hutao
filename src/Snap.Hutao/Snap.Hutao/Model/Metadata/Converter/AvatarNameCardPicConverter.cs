// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Data;

namespace Snap.Hutao.Model.Metadata.Converter;

/// <summary>
/// 角色名片转换器
/// </summary>
internal class AvatarNameCardPicConverter : IValueConverter
{
    private const string BaseUrl = "https://static.snapgenshin.com/NameCardPic/UI_NameCardPic_{0}_P.png";

    /// <inheritdoc/>
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value == null)
        {
            return null!;
        }

        Avatar.Avatar avatar = (Avatar.Avatar)value;
        string avatarName = ReplaceSpecialCaseNaming(avatar.Icon[14..]);
        return new Uri(string.Format(BaseUrl, avatarName));
    }

    private static string ReplaceSpecialCaseNaming(string avatarName)
    {
        return avatarName switch
        {
            "Yae" => "Yae1",
            _ => avatarName,
        };
    }

    /// <inheritdoc/>
    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw Must.NeverHappen();
    }
}