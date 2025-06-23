// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Documents;
using Snap.Hutao.UI.Xaml.Control.TextBlock.Syntax.MiHoYo;

namespace Snap.Hutao.UI.Xaml.Control.TextBlock;

[DependencyProperty("LinkData", typeof(Tuple<MiHoYoSyntaxLinkKind, uint>), default(Tuple<MiHoYoSyntaxLinkKind, uint>), AttachedType = typeof(Hyperlink), IsAttached = true)]
internal partial class DescriptionHyperLinkHelper
{
}