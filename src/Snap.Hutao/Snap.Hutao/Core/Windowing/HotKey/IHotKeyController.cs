// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Core.Windowing.HotKey;

internal interface IHotKeyController
{
    void OnHotKeyPressed(in HotKeyParameter parameter);

    void RegisterAll();

    void UnregisterAll();
}