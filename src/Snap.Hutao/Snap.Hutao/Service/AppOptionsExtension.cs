// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.IO;

namespace Snap.Hutao.Service;

internal static class AppOptionsExtension
{
    public static bool TryGetGameFolderAndFileName(this AppOptions appOptions, [NotNullWhen(true)] out string? gameFolder, [NotNullWhen(true)] out string? gameFileName)
    {
        string gamePath = appOptions.GamePath;

        gameFolder = Path.GetDirectoryName(gamePath);
        if (string.IsNullOrEmpty(gameFolder))
        {
            gameFileName = default;
            return false;
        }

        gameFileName = Path.GetFileName(gamePath);
        if (string.IsNullOrEmpty(gameFileName))
        {
            return false;
        }

        return true;
    }

    public static bool TryGetGameFileName(this AppOptions appOptions, [NotNullWhen(true)] out string? gameFileName)
    {
        string gamePath = appOptions.GamePath;

        gameFileName = Path.GetFileName(gamePath);
        if (string.IsNullOrEmpty(gameFileName))
        {
            return false;
        }

        return true;
    }
}