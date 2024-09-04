// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Snap.Hutao.Core.IO;

namespace Snap.Hutao.Core.Caching;

internal readonly struct ElementThemeValueFile
{
    public readonly ValueFile File;
    public readonly ElementTheme Theme;

    public ElementThemeValueFile(ValueFile file, ElementTheme theme)
    {
        File = file;
        Theme = theme;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(File, Theme);
    }
}