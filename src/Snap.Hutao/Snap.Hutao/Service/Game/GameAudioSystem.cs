// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.IO;

namespace Snap.Hutao.Service.Game;

internal sealed class GameAudioSystem
{
    public GameAudioSystem(string gameFilePath)
    {
        string? directory = Path.GetDirectoryName(gameFilePath);
        ArgumentException.ThrowIfNullOrEmpty(directory);

        Chinese = File.Exists(Path.Combine(directory, GameConstants.AudioChinesePkgVersion));
        English = File.Exists(Path.Combine(directory, GameConstants.AudioEnglishPkgVersion));
        Japanese = File.Exists(Path.Combine(directory, GameConstants.AudioJapanesePkgVersion));
        Korean = File.Exists(Path.Combine(directory, GameConstants.AudioKoreanPkgVersion));
    }

    public GameAudioSystem(bool chinese, bool english, bool japanese, bool korean)
    {
        Chinese = chinese;
        English = english;
        Japanese = japanese;
        Korean = korean;
    }

    public bool Chinese { get; }

    public bool English { get; }

    public bool Japanese { get; }

    public bool Korean { get; }
}