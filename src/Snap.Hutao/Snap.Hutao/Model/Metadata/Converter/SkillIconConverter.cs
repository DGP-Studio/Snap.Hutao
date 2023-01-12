// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Control;

namespace Snap.Hutao.Model.Metadata.Converter;

/// <summary>
/// 技能图标转换器
/// </summary>
internal class SkillIconConverter : ValueConverter<string, Uri>
{
    /// <summary>
    /// 名称转Uri
    /// </summary>
    /// <param name="name">名称</param>
    /// <returns>链接</returns>
    public static Uri IconNameToUri(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return Web.HutaoEndpoints.UIIconNone;
        }

        if (name.StartsWith("UI_Talent_"))
        {
            return new Uri(Web.HutaoEndpoints.StaticFile("Talent", $"{name}.png"));
        }
        else
        {
            return new Uri(Web.HutaoEndpoints.StaticFile("Skill", $"{name}.png"));
        }
    }

    /// <inheritdoc/>
    public override Uri Convert(string from)
    {
        return IconNameToUri(from);
    }
}