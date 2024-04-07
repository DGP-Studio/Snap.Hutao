// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Media.Animation;
using Snap.Hutao.Core.Abstraction;
using Snap.Hutao.Core.Abstraction.Extension;
using System.Collections.ObjectModel;
using Windows.Foundation;

namespace Snap.Hutao.Service.Notification;

internal sealed class InfoBarOptions
{
    public InfoBarSeverity Severity { get; set; }

    public string? Title { get; set; }

    public string? Message { get; set; }

    public object? Content { get; set; }

    public ButtonBase? ActionButton { get; set; }

    public int MilliSecondsDelay { get; set; }
}