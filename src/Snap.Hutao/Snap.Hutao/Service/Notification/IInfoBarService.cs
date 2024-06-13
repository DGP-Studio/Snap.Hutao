// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Collections.ObjectModel;

namespace Snap.Hutao.Service.Notification;

[HighQuality]
internal interface IInfoBarService
{
    ObservableCollection<InfoBarOptions> Collection { get; }

    void PrepareInfoBarAndShow(Action<IInfoBarOptionsBuilder> configure);
}
