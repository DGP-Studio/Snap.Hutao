// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32;
using Windows.UI;

namespace Snap.Hutao.UI.Xaml.Control.Theme;

internal static class SystemColors
{
    public static Color BaseLowColor(bool isDarkMode)
    {
        return isDarkMode ? StructMarshal.Color(0x33FFFFFF) : StructMarshal.Color(0x33000000);
    }

    public static Color BaseMediumLowColor(bool isDarkMode)
    {
        return isDarkMode ? StructMarshal.Color(0x66FFFFFF) : StructMarshal.Color(0x66000000);
    }

    public static Color BaseHighColor(bool isDarkMode)
    {
        return isDarkMode ? StructMarshal.Color(0xFFFFFFFF) : StructMarshal.Color(0xFF000000);
    }
}
