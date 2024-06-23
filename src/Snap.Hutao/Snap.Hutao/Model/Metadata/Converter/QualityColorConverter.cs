// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI;
using Snap.Hutao.Model.Intrinsic;
using Snap.Hutao.UI.Xaml.Control.Theme;
using Snap.Hutao.UI.Xaml.Data;
using System.Collections.Frozen;
using Windows.UI;

namespace Snap.Hutao.Model.Metadata.Converter;

/// <summary>
/// 品质颜色转换器
/// </summary>
[HighQuality]
internal sealed class QualityColorConverter : ValueConverter<QualityType, Color>
{
    private static readonly FrozenDictionary<string, QualityType> LocalizedNameToQualityType = FrozenDictionary.ToFrozenDictionary(
    [
        KeyValuePair.Create(SH.ModelIntrinsicItemQualityWhite, QualityType.QUALITY_WHITE),
        KeyValuePair.Create(SH.ModelIntrinsicItemQualityGreen, QualityType.QUALITY_GREEN),
        KeyValuePair.Create(SH.ModelIntrinsicItemQualityBlue, QualityType.QUALITY_BLUE),
        KeyValuePair.Create(SH.ModelIntrinsicItemQualityPurple, QualityType.QUALITY_PURPLE),
        KeyValuePair.Create(SH.ModelIntrinsicItemQualityOrange, QualityType.QUALITY_ORANGE),
        KeyValuePair.Create(SH.ModelIntrinsicItemQualityRed, QualityType.QUALITY_ORANGE_SP),
    ]);

    private static readonly FrozenDictionary<QualityType, Color> QualityTypeToColor = FrozenDictionary.ToFrozenDictionary(
    [
        KeyValuePair.Create(QualityType.QUALITY_WHITE, KnownColors.White),
        KeyValuePair.Create(QualityType.QUALITY_GREEN, KnownColors.Green),
        KeyValuePair.Create(QualityType.QUALITY_BLUE, KnownColors.Blue),
        KeyValuePair.Create(QualityType.QUALITY_PURPLE, KnownColors.Purple),
        KeyValuePair.Create(QualityType.QUALITY_ORANGE, KnownColors.Orange),
        KeyValuePair.Create(QualityType.QUALITY_ORANGE_SP, KnownColors.Orange),
    ]);

    public static Color QualityNameToColor(string qualityName)
    {
        return QualityToColor(LocalizedNameToQualityType.GetValueOrDefault(qualityName));
    }

    public static Color QualityToColor(QualityType quality)
    {
        return QualityTypeToColor.GetValueOrDefault(quality, Colors.Transparent);
    }

    /// <inheritdoc/>
    public override Color Convert(QualityType from)
    {
        return QualityToColor(from);
    }
}