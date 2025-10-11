// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Service.Notification.ToastAbstraction.Adaptive.Element;

namespace Snap.Hutao.Service.Notification.ToastAbstraction.Adaptive;

internal static class BaseImageHelper
{
    internal static void SetSource(ref string destination, string value)
    {
        ArgumentNullException.ThrowIfNull(value);
        destination = value;
    }

    internal static ElementAdaptiveImage CreateBaseElement(IBaseImage curr)
    {
        if (curr.Source is null)
        {
            throw new ArgumentException("Source property is required.");
        }

        return new()
        {
            Src = curr.Source,
            Alt = curr.AlternateText,
            AddImageQuery = curr.AddImageQuery
        };
    }
}