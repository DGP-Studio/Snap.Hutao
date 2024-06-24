// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Intrinsic;
using Snap.Hutao.UI.Xaml.Data.Converter;

namespace Snap.Hutao.Model.Metadata.Converter;

/// <summary>
/// 物品等级转换器
/// </summary>
[HighQuality]
internal sealed class QualityConverter : ValueConverter<QualityType, Uri>
{
    /// <inheritdoc/>
    public override Uri Convert(QualityType from)
    {
        string name = Enum.GetName(from) ?? from.ToString();
        if (name == nameof(QualityType.QUALITY_ORANGE_SP))
        {
            name = "QUALITY_RED";
        }

        return Web.HutaoEndpoints.StaticRaw("Bg", $"UI_{name}.png").ToUri();
    }
}
