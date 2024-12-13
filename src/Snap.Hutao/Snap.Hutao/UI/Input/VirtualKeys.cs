// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model;
using Snap.Hutao.Win32.UI.Input.KeyboardAndMouse;
using System.Collections.Immutable;

namespace Snap.Hutao.UI.Input;

internal static class VirtualKeys
{
    public static ImmutableArray<NameValue<VIRTUAL_KEY>> Values { get; } = ImmutableCollectionsNameValue.FromEnum<VIRTUAL_KEY>();

    public static NameValue<VIRTUAL_KEY> First(VIRTUAL_KEY value)
    {
        // The value may come from the result of a method call, so this method
        // is intentionally made to avoid multiple calls to compute input value
        return Values.First(n => n.Value == value);
    }
}