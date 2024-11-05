// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Intrinsic;

namespace Snap.Hutao.UI.Xaml.Control;

[DependencyProperty("Quality", typeof(QualityType), QualityType.QUALITY_NONE)]
[DependencyProperty("Icon", typeof(Uri))]
[DependencyProperty("IconOpacity", typeof(double), 1.0)]
[DependencyProperty("Badge", typeof(Uri))]
internal sealed partial class ItemIcon : Microsoft.UI.Xaml.Controls.Control;