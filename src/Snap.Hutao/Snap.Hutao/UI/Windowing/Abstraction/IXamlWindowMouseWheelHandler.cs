// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.UI.Windowing.Abstraction;

internal interface IXamlWindowMouseWheelHandler
{
    void OnMouseWheel(ref readonly PointerPointProperties data);
}