// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI;
using Snap.Hutao.Control;
using Snap.Hutao.Model.Intrinsic;
using Snap.Hutao.Win32;
using Windows.UI;

namespace Snap.Hutao.Model.Metadata.Converter;

/// <summary>
/// 品质颜色转换器
/// </summary>
[HighQuality]
internal sealed class QualityColorConverter : ValueConverter<ItemQuality, Color>
{
    /// <inheritdoc/>
    public override Color Convert(ItemQuality from)
    {
        return from switch
        {
            ItemQuality.QUALITY_WHITE => StructMarshal.Color(0xFF72778B),
            ItemQuality.QUALITY_GREEN => StructMarshal.Color(0xFF2A8F72),
            ItemQuality.QUALITY_BLUE => StructMarshal.Color(0xFF5180CB),
            ItemQuality.QUALITY_PURPLE => StructMarshal.Color(0xFFA156E0),
            ItemQuality.QUALITY_ORANGE or ItemQuality.QUALITY_ORANGE_SP => StructMarshal.Color(0xFFBC6932),
            _ => Colors.Transparent,
        };
    }
}