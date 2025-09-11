// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Service.Game.Package.Advanced;

namespace Snap.Hutao.Service.Game.FileSystem;

internal sealed partial class GameFileSystem : IGameFileSystem
{
    private readonly AsyncReaderWriterLock.Releaser releaser;

    public GameFileSystem(string gameFilePath, AsyncReaderWriterLock.Releaser releaser)
    {
        GameFilePath = gameFilePath;
        this.releaser = releaser;
    }

    public string GameFilePath { get; }

    [field: MaybeNull]
    public GameAudioInstallation Audio { get => field ??= new(this.GetGameDirectory()); }

    public bool IsDisposed { get; private set; }

    public static IGameFileSystem Create(string gameFilePath, AsyncReaderWriterLock.Releaser releaser)
    {
        return new GameFileSystem(gameFilePath, releaser);
    }

    public static IGameFileSystem CreateForPackageOperation(string gameFilePath, GameAudioInstallation? gameAudioSystem = default)
    {
        return new PackageOperationGameFileSystem(gameFilePath, gameAudioSystem);
    }

    public void Dispose()
    {
        if (IsDisposed)
        {
            return;
        }

        releaser.Dispose();
        IsDisposed = true;
    }
}