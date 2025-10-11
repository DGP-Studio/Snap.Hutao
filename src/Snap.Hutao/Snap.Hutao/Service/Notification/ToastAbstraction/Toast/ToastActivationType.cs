// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Service.Notification.ToastAbstraction.Common;

namespace Snap.Hutao.Service.Notification.ToastAbstraction.Toast;

/// <summary>
/// Decides the type of activation that will be used when the user interacts with the toast notification.
/// </summary>
internal enum ToastActivationType
{
    /// <summary>
    /// Default value. Your foreground app is launched.
    /// </summary>
    Foreground,

    /// <summary>
    /// Your corresponding background task (assuming you set everything up) is triggered, and you can execute code in the background (like sending the user's quick reply message) without interrupting the user.
    /// </summary>
    [EnumString("background")]
    Background,

    /// <summary>
    /// Launch a different app using protocol activation.
    /// </summary>
    [EnumString("protocol")]
    Protocol
}