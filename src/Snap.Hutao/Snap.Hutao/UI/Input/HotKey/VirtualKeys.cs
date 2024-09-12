// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model;
using Windows.System;

namespace Snap.Hutao.UI.Input.HotKey;

internal static class VirtualKeys
{
    private static readonly List<NameValue<VirtualKey>> Values = CollectionsNameValue.FromEnum<VirtualKey>();

    public static List<NameValue<VirtualKey>> GetList()
    {
        return Values;
    }
}