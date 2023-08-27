// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Snap.Hutao.View.Helper;

/// <summary>
/// 导航项帮助类
/// This type can't be internal
/// </summary>
[HighQuality]
[SuppressMessage("", "SH001")]
[DependencyProperty("NavigateTo", typeof(Type), IsAttached = true, AttachedType = typeof(NavigationViewItem))]
[DependencyProperty("ExtraData", typeof(object), IsAttached = true, AttachedType = typeof(NavigationViewItem))]
public sealed partial class NavHelper
{
}