// // Copyright (c) DGP Studio. All rights reserved.
// // Licensed under the MIT license.

using Snap.Hutao.Service.Game.Scheme;
using System.IO;

namespace Snap.Hutao.Service.Game.Package.Advanced;

internal sealed partial class PackageOperationGameFileSystem : IGameFileSystem
{
    public PackageOperationGameFileSystem(string gameFilePath, GameAudioSystem? gameAudioSystem = default)
    {
        GameFilePath = gameFilePath;
        Audio = gameAudioSystem ?? new(this.GetGameDirectory());
    }

    public string GameFilePath { get; }

    public GameAudioSystem Audio { get; }

    public bool IsDisposed { get; private set; }

    public void Dispose()
    {
        IsDisposed = true;
    }
}