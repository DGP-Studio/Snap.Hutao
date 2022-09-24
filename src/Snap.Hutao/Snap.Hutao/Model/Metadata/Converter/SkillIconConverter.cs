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

    /// <inheritdoc/>
    public override Uri Convert(string from)
    {
        if (from.StartsWith("UI_Talent_"))
        {
            return new Uri(string.Format(TalentUrl, from));
        }
        else
        {
            return new Uri(string.Format(SkillUrl, from));
        }
    }
}