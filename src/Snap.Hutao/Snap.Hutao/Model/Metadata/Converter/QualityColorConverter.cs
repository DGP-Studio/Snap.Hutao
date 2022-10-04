// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Control;
using Snap.Hutao.Model.Intrinsic;
using Windows.UI;

namespace Snap.Hutao.Model.Metadata.Converter;

/// <summary>
/// 品质颜色转换器
/// </summary>
internal class QualityColorConverter : ValueConverterBase<ItemQuality, Color>
{
    /// <inheritdoc/>
    public override Color Convert(ItemQuality from)
    {
        return from switch
        {
            ItemQuality.QUALITY_WHITE => Color.FromArgb(0xFF, 0x72, 0x77, 0x8B),
            ItemQuality.QUALITY_GREEN => Color.FromArgb(0xFF, 0x2A, 0x8F, 0x72),
            ItemQuality.QUALITY_BLUE => Color.FromArgb(0xFF, 0x51, 0x80, 0xCB),
            ItemQuality.QUALITY_PURPLE => Color.FromArgb(0xFF, 0xA1, 0x56, 0xE0),
            ItemQuality.QUALITY_ORANGE or ItemQuality.QUALITY_ORANGE_SP => Color.FromArgb(0xFF, 0xBC, 0x69, 0x32),
            _ => Color.FromArgb(0x00, 0x00, 0x00, 0x00),
        };
    }
}