﻿// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;

namespace Snap.Hutao.UI.Xaml;

/// <summary>
/// 绑定探针
/// 用于处理特定情况下需要穿透数据上下文的工作
/// DependencyObject will dispose inner ReferenceTracker in any time
/// when object is not used anymore.
/// </summary>
[HighQuality]
[DependencyProperty("DataContext", typeof(object))]
internal sealed partial class BindingProxy : DependencyObject;