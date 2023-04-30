// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Control;
using Snap.Hutao.Model.Intrinsic;

namespace Snap.Hutao.Model.Metadata.Converter;

/// <summary>
/// 物品等级转换器
/// </summary>
[HighQuality]
internal sealed class QualityConverter : ValueConverter<ItemQuality, Uri>
{
    /// <inheritdoc/>
    public override Uri Convert(ItemQuality from)
    {
        string name = Enum.GetName(from) ?? from.ToString();
        if (name == nameof(ItemQuality.QUALITY_ORANGE_SP))
        {
            name = "QUALITY_RED";
        }

        return Web.HutaoEndpoints.StaticFile("Bg", $"UI_{name}.png").ToUri();
    }
}
