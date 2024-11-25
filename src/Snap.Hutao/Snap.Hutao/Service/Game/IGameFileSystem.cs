// // Copyright (c) DGP Studio. All rights reserved.
// // Licensed under the MIT license.

namespace Snap.Hutao.Service.Game;

internal interface IGameFileSystem : IDisposable
{
    string GameFilePath { get; }

    string GameFileName { get; }

    string GameDirectory { get; }

    string GameConfigFilePath { get; }

    // ReSharper disable once InconsistentNaming
    string PCGameSDKFilePath { get; }

    string ScreenShotDirectory { get; }

    string DataDirectory { get; }

    string ScriptVersionFilePath { get; }

    string ChunksDirectory { get; }

    string PredownloadStatusPath { get; }

    GameAudioSystem Audio { get; }
}