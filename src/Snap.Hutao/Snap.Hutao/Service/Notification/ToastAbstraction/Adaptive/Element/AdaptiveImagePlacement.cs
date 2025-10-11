// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Service.Notification.ToastAbstraction.Common;

namespace Snap.Hutao.Service.Notification.ToastAbstraction.Adaptive.Element;

internal enum AdaptiveImagePlacement
{
    [EnumString("inline")]
    Inline,

    [EnumString("background")]
    Background,

    [EnumString("peek")]
    Peek,

    [EnumString("hero")]
    Hero,

    [EnumString("appLogoOverride")]
    AppLogoOverride
}