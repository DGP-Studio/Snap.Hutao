// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Controls;

namespace Snap.Hutao.UI.Xaml.Control;

[SuppressMessage("", "SH001")]
[DependencyProperty<bool>("IsTextSelectionEnabled", DefaultValue = false, IsAttached = true, TargetType = typeof(InfoBar), NotNull = true)]
public sealed partial class InfoBarHelper;