// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Net;

namespace Snap.Hutao.Service.Notification.ToastAbstraction.Common;

internal static class Util
{
    public const int NotificationContentVersion = 1;

    public static string HttpEncode(string value)
    {
        return WebUtility.HtmlEncode(value);
    }
}