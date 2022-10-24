// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Control;

namespace Snap.Hutao.Model.Metadata.Converter;

/// <summary>
/// 技能图标转换器
/// </summary>
internal class SkillIconConverter : ValueConverterBase<string, Uri>
{
    private const string SkillUrl = "https://static.snapgenshin.com/Skill/{0}.png";
    private const string TalentUrl = "https://static.snapgenshin.com/Talent/{0}.png";

    private static readonly Uri UIIconNone = new("https://static.snapgenshin.com/Bg/UI_Icon_None.png");

    /// <summary>
    /// 名称转Uri
    /// </summary>
    /// <param name="name">名称</param>
    /// <returns>链接</returns>
    public static Uri IconNameToUri(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return UIIconNone;
        }

        if (name.StartsWith("UI_Talent_"))
        {
            return new Uri(string.Format(TalentUrl, name));
        }
        else
        {
            return new Uri(string.Format(SkillUrl, name));
        }
    }

    /// <inheritdoc/>
    public override Uri Convert(string from)
    {
        return IconNameToUri(from);
    }
}