// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Data;

namespace Snap.Hutao.Model.Metadata.Converter;

/// <summary>
/// 技能图标转换器
/// </summary>
internal class SkillIconConverter : IValueConverter
{
    private const string SkillUrl = "https://static.snapgenshin.com/Skill/{0}.png";
    private const string TalentUrl = "https://static.snapgenshin.com/Talent/{0}.png";

    /// <inheritdoc/>
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        string target = (string)value;

        if (target.StartsWith("UI_Talent_"))
        {
            return new Uri(string.Format(TalentUrl, target));
        }
        else
        {
            return new Uri(string.Format(SkillUrl, target));
        }
    }

    /// <inheritdoc/>
    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw Must.NeverHappen();
    }
}