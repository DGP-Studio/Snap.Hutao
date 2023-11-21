// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Snap.Hutao.Control.Helper;

[SuppressMessage("", "SH001")]
[DependencyProperty("LeftPanelMaxWidth", typeof(double), IsAttached = true, AttachedType = typeof(ScrollViewer))]
[DependencyProperty("RightPanel", typeof(UIElement), IsAttached = true, AttachedType = typeof(ScrollViewer))]
public sealed partial class ScrollViewerHelper
{
}