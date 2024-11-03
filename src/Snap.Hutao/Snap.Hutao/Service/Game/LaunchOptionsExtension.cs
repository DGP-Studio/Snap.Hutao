// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Service.Game.PathAbstraction;
using System.Collections.Immutable;
using System.Runtime.InteropServices;

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

        fileSystem = new(gamePath);
        return true;
    }

    public static ImmutableArray<GamePathEntry> GetGamePathEntries(this LaunchOptions options, out GamePathEntry? selected)
    {
        string gamePath = options.GamePath;

        if (string.IsNullOrEmpty(gamePath))
        {
            selected = default;
            return options.GamePathEntries;
        }

        if (options.GamePathEntries.SingleOrDefault(entry => string.Equals(entry.Path, options.GamePath, StringComparison.OrdinalIgnoreCase)) is { } existed)
        {
            selected = existed;
            return options.GamePathEntries;
        }

        selected = GamePathEntry.Create(options.GamePath);
        return options.GamePathEntries = options.GamePathEntries.Add(selected);
    }

    public static ImmutableArray<GamePathEntry> RemoveGamePathEntry(this LaunchOptions options, GamePathEntry? entry, out GamePathEntry? selected)
    {
        if (entry is null)
        {
            return options.GetGamePathEntries(out selected);
        }

        if (string.Equals(options.GamePath, entry.Path, StringComparison.OrdinalIgnoreCase))
        {
            options.GamePath = string.Empty;
        }

        options.GamePathEntries = options.GamePathEntries.Remove(entry);
        return options.GetGamePathEntries(out selected);
    }

    public static ImmutableArray<GamePathEntry> UpdateGamePath(this LaunchOptions options, string gamePath)
    {
        options.GamePath = gamePath;
        return options.GetGamePathEntries(out _);
    }
}