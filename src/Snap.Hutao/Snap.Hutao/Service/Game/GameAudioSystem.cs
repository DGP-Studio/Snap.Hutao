// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.IO;

namespace Snap.Hutao.Service.Game;

internal sealed class GameAudioSystem
{
    private readonly string gameDirectory;

    public GameAudioSystem(string gameFilePath)
    {
        string? directory = Path.GetDirectoryName(gameFilePath);
        ArgumentException.ThrowIfNullOrEmpty(directory);
        gameDirectory = directory;
    }

    public string GameDirectory { get => gameDirectory; }

    public bool Chinese { get => File.Exists(Path.Combine(GameDirectory, GameConstants.AudioChinesePkgVersion)); }

    public bool English { get => File.Exists(Path.Combine(GameDirectory, GameConstants.AudioEnglishPkgVersion)); }

    public bool Japanese { get => File.Exists(Path.Combine(GameDirectory, GameConstants.AudioJapanesePkgVersion)); }

    public bool Korean { get => File.Exists(Path.Combine(GameDirectory, GameConstants.AudioKoreanPkgVersion)); }
}
