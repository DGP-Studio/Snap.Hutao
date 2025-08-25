// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Documents;
using Snap.Hutao.UI.Xaml.Control.TextBlock.Syntax.MiHoYo;

namespace Snap.Hutao.UI.Xaml.Control.TextBlock;

[DependencyProperty<Tuple<MiHoYoSyntaxLinkKind, uint>>("LinkData", IsAttached = true, TargetType = typeof(Hyperlink))]
internal partial class DescriptionHyperLinkHelper
{
}