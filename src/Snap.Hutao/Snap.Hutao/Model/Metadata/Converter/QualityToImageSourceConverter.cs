// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Media;
using Snap.Hutao.Model.Intrinsic;
using Snap.Hutao.UI.Xaml.Data.Converter;

namespace Snap.Hutao.Model.Metadata.Converter;

[DependencyProperty<ImageSource>("RedSource")]
[DependencyProperty<ImageSource>("OrangeSource")]
[DependencyProperty<ImageSource>("PurpleSource")]
[DependencyProperty<ImageSource>("BlueSource")]
[DependencyProperty<ImageSource>("GreenSource")]
[DependencyProperty<ImageSource>("WhiteSource")]
[DependencyProperty<ImageSource>("NoneSource")]
internal sealed partial class QualityToImageSourceConverter : DependencyValueConverter<QualityType, ImageSource?>
{
    public override ImageSource? Convert(QualityType from)
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