// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Service.Notification.ToastAbstraction.Common;

/// <summary>
/// Exception returned when invalid notification content is provided.
/// </summary>
internal sealed class NotificationContentValidationException : Exception
{
    public NotificationContentValidationException(string message)
        : base(message)
    {
    }
}