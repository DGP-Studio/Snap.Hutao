// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Control;
using Snap.Hutao.Model.Intrinsic;

namespace Snap.Hutao.Model.Metadata.Converter;

/// <summary>
/// 物品等级转换器
/// </summary>
internal class QualityConverter : ValueConverterBase<ItemQuality, Uri>
{
    /// <inheritdoc/>
    public override Uri Convert(ItemQuality from)
    {
        string? name = from.ToString();
        if (name == nameof(ItemQuality.QUALITY_ORANGE_SP))
        {
            name = "QUALITY_RED";
        }

        return new Uri(Web.HutaoEndpoints.StaticFile("Bg", $"UI_{name}.png"));
    }
}
