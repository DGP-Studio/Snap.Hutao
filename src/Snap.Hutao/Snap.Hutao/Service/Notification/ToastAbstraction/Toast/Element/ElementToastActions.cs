// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Service.Notification.ToastAbstraction.Common;

namespace Snap.Hutao.Service.Notification.ToastAbstraction.Toast.Element;

[NotificationXmlElement("actions")]
internal sealed class ElementToastActions
{
    internal const ToastSystemCommand DefaultSystemCommand = ToastSystemCommand.None;

    [NotificationXmlAttribute("hint-systemCommands", DefaultSystemCommand)]
    public ToastSystemCommand SystemCommands { get; set; }

    public IList<IElementToastActionsChild> Children { get; private set; } = [];
}