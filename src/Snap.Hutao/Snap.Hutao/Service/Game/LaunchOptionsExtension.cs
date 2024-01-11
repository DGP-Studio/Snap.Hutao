// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Service.Game.PathAbstraction;
using System.Collections.Immutable;

namespace Snap.Hutao.Service.Game;

internal static class LaunchOptionsExtension
{
    public static bool TryGetGameFileSystem(this LaunchOptions options, [NotNullWhen(true)] out GameFileSystem? fileSystem)
    {
        string gamePath = options.GamePath;

        if (string.IsNullOrEmpty(gamePath))
        {
            fileSystem = default;
            return false;
        }

        fileSystem = new GameFileSystem(gamePath);
        return true;
    }

    public static ImmutableList<GamePathEntry> GetGamePathEntries(this LaunchOptions options, out GamePathEntry? entry)
    {
        string gamePath = options.GamePath;

        if (string.IsNullOrEmpty(gamePath))
        {
            entry = default;
            return options.GamePathEntries;
        }

        if (options.GamePathEntries.SingleOrDefault(entry => string.Equals(entry.Path, options.GamePath, StringComparison.OrdinalIgnoreCase)) is { } existed)
        {
            entry = existed;
            return options.GamePathEntries;
        }

        entry = GamePathEntry.Create(options.GamePath);
        return [.. options.GamePathEntries, entry];
    }

    public static ImmutableList<GamePathEntry> RemoveGamePathEntry(this LaunchOptions options, GamePathEntry? entry, out GamePathEntry? selected)
    {
        if (entry is not null)
        {
            if (string.Equals(options.GamePath, entry.Path, StringComparison.OrdinalIgnoreCase))
            {
                options.GamePath = string.Empty;
            }

            options.GamePathEntries = options.GamePathEntries.Remove(entry);
        }

        return options.GetGamePathEntries(out selected);
    }

    public static ImmutableList<GamePathEntry> UpdateGamePathAndRefreshEntries(this LaunchOptions options, string gamePath)
    {
        options.GamePath = gamePath;
        ImmutableList<GamePathEntry> entries = options.GetGamePathEntries(out _);
        options.GamePathEntries = entries;
        return entries;
    }
}