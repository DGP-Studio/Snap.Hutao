// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Service.Notification.ToastAbstraction.Adaptive.Element;

namespace Snap.Hutao.Service.Notification.ToastAbstraction.Adaptive;

internal class BaseTextHelper
{
    internal static ElementAdaptiveText CreateBaseElement(IBaseText curr)
    {
        return new()
        {
            Text = curr.Text,
            Lang = curr.Language
        };
    }
}