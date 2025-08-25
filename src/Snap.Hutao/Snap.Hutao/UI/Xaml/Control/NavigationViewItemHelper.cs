// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Controls;

namespace Snap.Hutao.UI.Xaml.Control;

[SuppressMessage("", "SH001")]
[DependencyProperty<Type>("NavigateTo", IsAttached = true, TargetType = typeof(NavigationViewItem))]
[DependencyProperty<object>("ExtraData", IsAttached = true, TargetType = typeof(NavigationViewItem))]
public sealed partial class NavigationViewItemHelper;