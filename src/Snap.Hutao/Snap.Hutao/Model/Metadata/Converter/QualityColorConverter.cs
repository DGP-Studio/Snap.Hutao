// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI;
using Snap.Hutao.Control;
using Snap.Hutao.Core;
using Snap.Hutao.Model.Intrinsic;
using Windows.UI;

namespace Snap.Hutao.Model.Metadata.Converter;

/// <summary>
/// 品质颜色转换器
/// </summary>
[HighQuality]
internal sealed class QualityColorConverter : ValueConverter<QualityType, Color>
{
    /// <inheritdoc/>
    public override Color Convert(QualityType from)
    {
        return from switch
        {
            QualityType.QUALITY_WHITE => StructMarshal.Color(0xFF72778B),
            QualityType.QUALITY_GREEN => StructMarshal.Color(0xFF2A8F72),
            QualityType.QUALITY_BLUE => StructMarshal.Color(0xFF5180CB),
            QualityType.QUALITY_PURPLE => StructMarshal.Color(0xFFA156E0),
            QualityType.QUALITY_ORANGE or QualityType.QUALITY_ORANGE_SP => StructMarshal.Color(0xFFBC6932),
            _ => Colors.Transparent,
        };
    }
}