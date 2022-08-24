// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Data;

namespace Snap.Hutao.Model.Metadata.Converter;

/// <summary>
/// 角色侧面头像转换器
/// </summary>
internal class AvatarSideIconConverter : IValueConverter
{
    private const string BaseUrl = "https://static.snapgenshin.com/AvatarIcon/{0}.png";

    /// <inheritdoc/>
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        return new Uri(string.Format(BaseUrl, value));
    }

    /// <inheritdoc/>
    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw Must.NeverHappen();
    }
}