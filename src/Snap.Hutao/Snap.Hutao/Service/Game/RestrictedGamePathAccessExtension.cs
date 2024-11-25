// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.ExceptionService;
using Snap.Hutao.Service.Game.PathAbstraction;
using System.Collections.Immutable;

namespace Snap.Hutao.Service.Game;

internal static class RestrictedGamePathAccessExtension
{
    public static bool TryGetGameFileSystem(this IRestrictedGamePathAccess access, [NotNullWhen(true)] out IGameFileSystem? fileSystem)
    {
        string gamePath = access.GamePath;

        if (string.IsNullOrEmpty(gamePath))
        {
            fileSystem = default;
            return false;
        }

        if (!access.GamePathLock.TryReaderLock(out AsyncReaderWriterLock.Releaser releaser))
        {
            fileSystem = default;
            return false;
        }

        fileSystem = new GameFileSystem(gamePath, releaser);
        return true;
    }

    public static ImmutableArray<GamePathEntry> GetGamePathEntries(this IRestrictedGamePathAccess access, out GamePathEntry? selected)
    {
        string gamePath = access.GamePath;

        if (string.IsNullOrEmpty(gamePath))
        {
            selected = default;
            return access.GamePathEntries;
        }

        if (access.GamePathEntries.SingleOrDefault(entry => string.Equals(entry.Path, access.GamePath, StringComparison.OrdinalIgnoreCase)) is { } existed)
        {
            selected = existed;
            return access.GamePathEntries;
        }

        selected = GamePathEntry.Create(access.GamePath);
        return access.GamePathEntries = access.GamePathEntries.Add(selected);
    }

    public static ImmutableArray<GamePathEntry> RemoveGamePathEntry(this IRestrictedGamePathAccess access, GamePathEntry? entry, out GamePathEntry? selected)
    {
        if (entry is null)
        {
            return access.GetGamePathEntries(out selected);
        }

        if (!access.GamePathLock.TryWriterLock(out AsyncReaderWriterLock.Releaser releaser))
        {
            throw HutaoException.InvalidOperation("Cannot remove game path while it is being used.");
        }

        using (releaser)
        {
            if (string.Equals(access.GamePath, entry.Path, StringComparison.OrdinalIgnoreCase))
            {
                access.GamePath = string.Empty;
            }

            access.GamePathEntries = access.GamePathEntries.Remove(entry);
            return access.GetGamePathEntries(out selected);
        }
    }

    public static ImmutableArray<GamePathEntry> UpdateGamePath(this IRestrictedGamePathAccess access, string gamePath)
    {
        if (!access.GamePathLock.TryWriterLock(out AsyncReaderWriterLock.Releaser releaser))
        {
            throw HutaoException.InvalidOperation("Cannot update game path while it is being used.");
        }

        using (releaser)
        {
            access.GamePath = gamePath;
            return access.GetGamePathEntries(out _);
        }
    }
}