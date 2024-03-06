// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Snap.Hutao.Model;

namespace Snap.Hutao.Core.Windowing;

internal static class KnownElementThemes
{
    public static List<NameValue<ElementTheme>> Get()
    {
        return
        [
            new(SH.CoreWindowThemeLight, ElementTheme.Light),
            new(SH.CoreWindowThemeDark,ElementTheme.Dark),
            new(SH.CoreWindowThemeSystem, ElementTheme.Default),
        ];
    }
}
