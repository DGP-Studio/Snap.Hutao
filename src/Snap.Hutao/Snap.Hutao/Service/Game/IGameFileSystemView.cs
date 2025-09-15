// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Service.Game;

internal interface IGameFileSystemView
{
    bool IsDisposed { get; }

    string GameFilePath { get; }

    GameAudioInstallation Audio { get; }
}