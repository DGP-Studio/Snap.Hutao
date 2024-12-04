// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Intrinsic;
using Snap.Hutao.UI.Xaml.Data.Converter;
using Snap.Hutao.Web.Endpoint.Hutao;

namespace Snap.Hutao.Model.Metadata.Converter;

internal sealed partial class QualityConverter : ValueConverter<QualityType, Uri>
{
    public override Uri Convert(QualityType from)
    {
        string name = Enum.GetName(from) ?? from.ToString();
        if (name is nameof(QualityType.QUALITY_ORANGE_SP))
        {
            name = "QUALITY_RED";
        }

        return StaticResourcesEndpoints.StaticRaw("Bg", $"UI_{name}.png").ToUri();
    }
}