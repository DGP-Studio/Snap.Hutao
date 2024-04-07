// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using System.Collections.ObjectModel;
using Windows.Foundation;

namespace Snap.Hutao.Service.Notification;

internal sealed class InfoBarOptionsBuilder : IInfoBarOptionsBuilder
{
    public InfoBarOptions Options { get; } = new();
}