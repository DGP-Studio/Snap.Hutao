// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Service.Notification.ToastAbstraction.Common;

internal sealed class EnumStringAttribute : Attribute
{
    public string String { get; }

    public EnumStringAttribute(string s)
    {
        ArgumentNullException.ThrowIfNull(s);
        String = s;
    }

    public override string ToString()
    {
        return String;
    }
}