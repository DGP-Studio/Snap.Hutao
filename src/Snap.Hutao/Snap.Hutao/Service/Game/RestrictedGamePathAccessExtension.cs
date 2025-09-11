// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.ExceptionService;
using Snap.Hutao.Service.Game.PathAbstraction;
using System.Collections.Immutable;

namespace Snap.Hutao.Service.Game;

internal static class RestrictedGamePathAccessExtension
{
    [Obsolete]
    public static bool TryGetGameFileSystem(this IRestrictedGamePathAccess access, [NotNullWhen(true)] out IGameFileSystem? fileSystem)
    {
        string gamePath = access.GamePath.Value;

        if (string.IsNullOrEmpty(gamePath) || !access.GamePathLock.TryReaderLock(out AsyncReaderWriterLock.Releaser releaser))
        {
            fileSystem = default;
            return false;
        }

        fileSystem = GameFileSystem.Create(gamePath, releaser);
        return true;
    }

    // TODO: implement tracing
    public static GameFileSystemErrorKind TryGetGameFileSystem(this IRestrictedGamePathAccess access, string trace, out IGameFileSystem? fileSystem)
    {
        string gamePath = access.GamePath.Value;

        if (string.IsNullOrEmpty(gamePath))
        {
            fileSystem = default;
            return GameFileSystemErrorKind.GamePathNullOrEmpty;
        }

        if (!access.GamePathLock.TryReaderLock(out AsyncReaderWriterLock.Releaser releaser))
        {
            fileSystem = default;
            return GameFileSystemErrorKind.GamePathLocked;
        }

        fileSystem = GameFileSystem.Create(gamePath, releaser);
        return GameFileSystemErrorKind.None;
    }

    public static ImmutableArray<GamePathEntry> PerformGamePathEntrySynchronization(this IRestrictedGamePathAccess access, out GamePathEntry? selected)
    {
        string gamePath = access.GamePath.Value;

        // The game path is null or empty, this means no game path is selected, just return the entries.
        if (string.IsNullOrEmpty(gamePath))
        {
            selected = default;
            return access.GamePathEntries.Value;
        }

        // The game path is in the entries, just return the entries.
        if (access.GamePathEntries.Value.SingleOrDefault(entry => string.Equals(entry.Path, gamePath, StringComparison.OrdinalIgnoreCase)) is { } existed)
        {
            selected = existed;
            return access.GamePathEntries.Value;
        }

        // We need update the entries when game path not in the entries.
        if (!access.GamePathLock.TryWriterLock(out AsyncReaderWriterLock.Releaser releaser))
        {
            throw HutaoException.InvalidOperation("Cannot set game path entries while it is being used.");
        }

        using (releaser)
        {
            // The game path is not in the entries, add it to the entries.
            selected = GamePathEntry.Create(access.GamePath.Value);
            return access.GamePathEntries.Value = access.GamePathEntries.Value.Add(selected);
        }
    }

    public static ImmutableArray<GamePathEntry> RemoveGamePathEntry(this IRestrictedGamePathAccess access, GamePathEntry? entry, out GamePathEntry? selected)
    {
        // Although normally this should not happen, we still handle it for compatibility.
        if (entry is null)
        {
            // Removes no entry, just return the entries.
            return access.PerformGamePathEntrySynchronization(out selected);
        }

        if (!access.GamePathLock.TryWriterLock(out AsyncReaderWriterLock.Releaser releaser))
        {
            throw HutaoException.InvalidOperation("Cannot remove game path while it is being used.");
        }

        using (releaser)
        {
            // Clear game path if it's selected.
            if (string.Equals(access.GamePath.Value, entry.Path, StringComparison.OrdinalIgnoreCase))
            {
                access.GamePath.Value = string.Empty;
            }

            access.GamePathEntries.Value = access.GamePathEntries.Value.Remove(entry);
        }

        // Synchronization takes write lock when game path changed,
        // so we release the write lock before calling.
        return access.PerformGamePathEntrySynchronization(out selected);
    }

    public static ImmutableArray<GamePathEntry> UpdateGamePath(this IRestrictedGamePathAccess access, string gamePath)
    {
        if (!access.GamePathLock.TryWriterLock(out AsyncReaderWriterLock.Releaser releaser))
        {
            throw HutaoException.InvalidOperation("Cannot update game path while it is being used.");
        }

        using (releaser)
        {
            access.GamePath.Value = gamePath;
        }

        // Synchronization takes write lock when game path changed,
        // so we release the write lock before calling.
        return access.PerformGamePathEntrySynchronization(out _);
    }
}

internal enum GameFileSystemErrorKind
{
    None,
    GamePathNullOrEmpty,
    GamePathLocked,
}