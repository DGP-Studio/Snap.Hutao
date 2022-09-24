// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Control;

namespace Snap.Hutao.Model.Metadata.Converter;

/// <summary>
/// 角色侧面头像转换器
/// </summary>
internal class AvatarSideIconConverter : ValueConverterBase<string, Uri>
{
    private const string BaseUrl = "https://static.snapgenshin.com/AvatarIcon/{0}.png";

    /// <inheritdoc/>
    public override Uri Convert(string from)
    {
        return new Uri(string.Format(BaseUrl, from));
    }
}