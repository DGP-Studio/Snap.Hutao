// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.ExceptionService;
using Snap.Hutao.Service.Game.Scheme;
using System.IO;

namespace Snap.Hutao.Service.Game;

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
    public GameAudioSystem Audio { get => field ??= new(GameFilePath); }

    public bool IsDisposed { get; private set; }

    public void Dispose()
    {
        releaser.Dispose();
        IsDisposed = true;
    }
}