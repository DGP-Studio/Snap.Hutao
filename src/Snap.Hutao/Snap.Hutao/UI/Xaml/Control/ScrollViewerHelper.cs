// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Snap.Hutao.UI.Xaml.Control;

[SuppressMessage("", "SH001")]
[DependencyProperty("RightPanel", typeof(UIElement), IsAttached = true, AttachedType = typeof(ScrollViewer))]
public sealed partial class ScrollViewerHelper;