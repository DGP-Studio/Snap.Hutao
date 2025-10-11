// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Service.Notification.ToastAbstraction.Common;

namespace Snap.Hutao.Service.Notification.ToastAbstraction.Toast.Element;

internal enum ToastImagePlacement
{
    Inline,

    [EnumString("appLogoOverride")]
    AppLogoOverride,

    [EnumString("hero")]
    Hero
}