// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Service.Notification.ToastAbstraction.Common;

internal sealed class NotificationXmlAttributeAttribute : Attribute
{
    public string Name { get; }

    public object? DefaultValue { get; }

    public NotificationXmlAttributeAttribute(string name, object? defaultValue = default)
    {
        Name = name;
        DefaultValue = defaultValue;
    }
}