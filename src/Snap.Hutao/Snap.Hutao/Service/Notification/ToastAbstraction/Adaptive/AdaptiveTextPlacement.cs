// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Service.Notification.ToastAbstraction.Common;

namespace Snap.Hutao.Service.Notification.ToastAbstraction.Adaptive;

internal enum AdaptiveTextPlacement
{
    /// <summary>
    /// Default value
    /// </summary>
    Inline,

    [EnumString("attribution")]
    Attribution
}