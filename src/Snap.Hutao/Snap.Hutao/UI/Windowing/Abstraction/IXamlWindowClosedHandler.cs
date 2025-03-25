// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.UI.Windowing.Abstraction;

internal interface IXamlWindowClosedHandler
{
    void OnWindowClosing(out bool cancel);

    void OnWindowClosed();
}