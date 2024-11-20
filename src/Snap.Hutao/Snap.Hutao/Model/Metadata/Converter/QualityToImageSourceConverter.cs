// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Media;
using Snap.Hutao.Model.Intrinsic;
using Snap.Hutao.UI.Xaml.Data.Converter;

namespace Snap.Hutao.Model.Metadata.Converter;

[DependencyProperty("RedSource", typeof(ImageSource))]
[DependencyProperty("OrangeSource", typeof(ImageSource))]
[DependencyProperty("PurpleSource", typeof(ImageSource))]
[DependencyProperty("BlueSource", typeof(ImageSource))]
[DependencyProperty("GreenSource", typeof(ImageSource))]
[DependencyProperty("WhiteSource", typeof(ImageSource))]
[DependencyProperty("NoneSource", typeof(ImageSource))]
internal sealed partial class QualityToImageSourceConverter : DependencyValueConverter<QualityType, ImageSource>
{
    public override ImageSource Convert(QualityType from)
    {
        return from switch
        {
            QualityType.QUALITY_ORANGE_SP => RedSource,
            QualityType.QUALITY_ORANGE => OrangeSource,
            QualityType.QUALITY_PURPLE => PurpleSource,
            QualityType.QUALITY_BLUE => BlueSource,
            QualityType.QUALITY_GREEN => GreenSource,
            QualityType.QUALITY_WHITE => WhiteSource,
            _ => NoneSource,
        };
    }
}