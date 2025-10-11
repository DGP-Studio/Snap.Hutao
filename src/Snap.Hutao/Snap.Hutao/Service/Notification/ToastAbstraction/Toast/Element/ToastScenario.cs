// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.
using Snap.Hutao.Service.Notification.ToastAbstraction.Common;

namespace Snap.Hutao.Service.Notification.ToastAbstraction.Toast.Element;

/// <summary>
/// Specifies the scenario, controlling behaviors about the toast.
/// </summary>
public enum ToastScenario
{
    /// <summary>
    /// The normal toast behavior. The toast appears for a short duration, and then automatically dismisses into Action Center.
    /// </summary>
    Default,

    /// <summary>
    /// Causes the toast to stay on-screen and expanded until the user takes action. Also causes a looping alarm sound to be selected by default.
    /// </summary>
    [EnumString("alarm")]
    Alarm,

    /// <summary>
    /// Causes the toast to stay on-screen and expanded until the user takes action.
    /// </summary>
    [EnumString("reminder")]
    Reminder,

    /// <summary>
    /// Causes the toast to stay on-screen and expanded until the user takes action (on Mobile this expands to full screen). Also causes a looping incoming call sound to be selected by default.
    /// </summary>
    [EnumString("incomingCall")]
    IncomingCall
}