// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Service.Game.FileSystem;

internal sealed partial class GameFileSystemReference : IGameFileSystem
{
    private IGameFileSystem reference;

    public GameFileSystemReference(IGameFileSystem reference)
    {
        this.reference = reference;
    }

    public bool IsDisposed { get => reference.IsDisposed; }

    public string GameFilePath { get => reference.GameFilePath; }

    public GameAudioInstallation Audio { get => reference.Audio; }

    public void Exchange(IGameFileSystem newReference)
    {
        if (newReference is GameFileSystemReference wrapper)
        {
            Exchange(wrapper.reference);
            return;
        }

        if (!ReferenceEquals(reference, newReference))
        {
            reference.Dispose();
            reference = newReference;
        }
    }

    public void Dispose()
    {
        reference.Dispose();
    }
}