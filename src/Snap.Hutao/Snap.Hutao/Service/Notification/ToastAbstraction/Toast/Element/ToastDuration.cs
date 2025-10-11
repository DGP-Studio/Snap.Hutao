// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Service.Notification.ToastAbstraction.Common;

namespace Snap.Hutao.Service.Notification.ToastAbstraction.Toast.Element;

/// <summary>
/// The amount of time the toast should display.
/// </summary>
internal enum ToastDuration
{
    /// <summary>
    /// Default value. Toast appears for a short while and then goes into Action Center.
    /// </summary>
    Short,

    /// <summary>
    /// Toast stays on-screen for longer, and then goes into Action Center.
    /// </summary>
    [EnumString("long")]
    Long
}