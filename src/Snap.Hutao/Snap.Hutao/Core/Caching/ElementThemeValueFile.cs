// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Snap.Hutao.Core.IO;

namespace Snap.Hutao.Core.Caching;

internal readonly struct ElementThemeValueFile
{
    public readonly ElementTheme Theme;
    public readonly ValueFile File;

    public ElementThemeValueFile(ElementTheme theme, ValueFile file)
    {
        File = file;
        Theme = theme;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Theme, File);
    }
}