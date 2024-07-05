// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.UI.WindowsAndMessaging;

namespace Snap.Hutao.UI.Windowing.Abstraction;

internal interface IXamlWindowSubclassMinMaxInfoHandler
{
    unsafe void HandleMinMaxInfo(ref MINMAXINFO info, double scalingFactor);
}