// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.UI.Input;

namespace Snap.Hutao.UI.Windowing.Abstraction;

internal interface IXamlWindowMouseWheelHandler
{
    void OnMouseWheel(ref readonly PointerPointProperties data);
}