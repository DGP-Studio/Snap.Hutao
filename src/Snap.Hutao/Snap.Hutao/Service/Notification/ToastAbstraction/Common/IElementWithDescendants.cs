// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Service.Notification.ToastAbstraction.Common;

internal interface IElementWithDescendants
{
    IEnumerable<IElement> Descendants();
}