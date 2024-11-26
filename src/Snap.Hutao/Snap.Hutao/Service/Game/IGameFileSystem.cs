// // Copyright (c) DGP Studio. All rights reserved.
// // Licensed under the MIT license.

namespace Snap.Hutao.Service.Game;

internal interface IGameFileSystem : IDisposable
{
    string GameFilePath { get; }

    GameAudioSystem Audio { get; }

    bool IsDisposed { get; }
}