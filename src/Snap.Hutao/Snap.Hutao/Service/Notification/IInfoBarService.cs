// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Controls;
using System.Collections.ObjectModel;

namespace Snap.Hutao.Service.Notification;

[HighQuality]
internal interface IInfoBarService
{
    ObservableCollection<InfoBar> Collection { get; }

    void PrepareInfoBarAndShow(InfoBarSeverity severity, string? title, string? message, int delay);
}
