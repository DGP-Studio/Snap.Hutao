// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.ExceptionService;
using Snap.Hutao.Service.Game.PathAbstraction;

namespace Snap.Hutao.Service.Game.FileSystem;

internal static class RestrictedGamePathAccessExtension
{
    [Obsolete]
    public static bool TryGetGameFileSystem(this IRestrictedGamePathAccess access, [NotNullWhen(true)] out IGameFileSystem? fileSystem)
    {
        string? gamePath = access.GamePathEntry.Value?.Path;

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
        string? gamePath = access.GamePathEntry.Value?.Path;

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

    // The return value is the final game path after synchronization
    public static string PerformGamePathEntrySynchronization(this IRestrictedGamePathAccess access)
    {
        string? gamePath = access.GamePathEntry.Value?.Path;

        // The game path is null or empty, this means no game path is selected
        if (string.IsNullOrEmpty(gamePath))
        {
            access.GamePathEntry.Value = default;
            return string.Empty;
        }

        // The game path is in the entries
        if (access.GamePathEntries.Value.SingleOrDefault(entry => string.Equals(entry.Path, gamePath, StringComparison.OrdinalIgnoreCase)) is { } existed)
        {
            // Prefer the existed entry value
            access.GamePathEntry.Value = existed;
            return existed.Path;
        }

        // We need update the entries when game path not in the entries.
        if (!access.GamePathLock.TryWriterLock(out AsyncReaderWriterLock.Releaser releaser))
        {
            throw HutaoException.InvalidOperation("Cannot set game path entries while it is being used.");
        }

        using (releaser)
        {
            // The game path is not in the entries, add it to the entries.
            GamePathEntry newEntry = GamePathEntry.Create(gamePath);
            access.GamePathEntries.Value = access.GamePathEntries.Value.Add(newEntry);
            access.GamePathEntry.Value = newEntry;
        }

        return gamePath;
    }

    public static string RemoveGamePathEntry(this IRestrictedGamePathAccess access, GamePathEntry? entry)
    {
        if (entry is not null)
        {
            if (!access.GamePathLock.TryWriterLock(out AsyncReaderWriterLock.Releaser releaser))
            {
                throw HutaoException.InvalidOperation("Cannot remove game path while it is being used.");
            }

            using (releaser)
            {
                // Clear game path if it's selected.
                if (string.Equals(access.GamePathEntry.Value?.Path, entry.Path, StringComparison.OrdinalIgnoreCase))
                {
                    access.GamePathEntry.Value = default;
                }

                access.GamePathEntries.Value = access.GamePathEntries.Value.Remove(entry);
            }
        }

        return access.PerformGamePathEntrySynchronization();
    }

    public static string UpdateGamePath(this IRestrictedGamePathAccess access, string gamePath)
    {
        if (!access.GamePathLock.TryWriterLock(out AsyncReaderWriterLock.Releaser releaser))
        {
            throw HutaoException.InvalidOperation("Cannot update game path while it is being used.");
        }

        using (releaser)
        {
            access.GamePathEntry.Value = GamePathEntry.Create(gamePath);
        }

        // Synchronization takes write lock when game path changed,
        // so we release the write lock before calling.
        return access.PerformGamePathEntrySynchronization();
    }

    public static IGameFileSystem UnsafeForceUpdateGamePath(this IRestrictedGamePathAccess access, string gamePath, IGameFileSystem old)
    {
        old.Dispose();
        access.UpdateGamePath(gamePath);
        access.TryGetGameFileSystem($"{nameof(RestrictedGamePathAccessExtension)}.{nameof(UnsafeForceUpdateGamePath)}", out IGameFileSystem? newFileSystem);
        ArgumentNullException.ThrowIfNull(newFileSystem);
        return newFileSystem;
    }
}