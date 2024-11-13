// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model;
using Snap.Hutao.Win32.UI.Input.KeyboardAndMouse;

namespace Snap.Hutao.UI.Input;

internal static class VirtualKeys
{
    private static readonly List<NameValue<VIRTUAL_KEY>> Values = CollectionsNameValue.FromEnum<VIRTUAL_KEY>();

    public static List<NameValue<VIRTUAL_KEY>> GetList()
    {
        return Values;
    }
}