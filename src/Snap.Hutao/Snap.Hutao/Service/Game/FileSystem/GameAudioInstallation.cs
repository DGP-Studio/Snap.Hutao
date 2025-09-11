// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.IO;

namespace Snap.Hutao.Service.Game.FileSystem;

internal sealed class GameAudioInstallation
{
    public GameAudioInstallation(string gameDirectory)
    {
        Chinese = File.Exists(Path.Combine(gameDirectory, GameConstants.AudioChinesePkgVersion));
        English = File.Exists(Path.Combine(gameDirectory, GameConstants.AudioEnglishPkgVersion));
        Japanese = File.Exists(Path.Combine(gameDirectory, GameConstants.AudioJapanesePkgVersion));
        Korean = File.Exists(Path.Combine(gameDirectory, GameConstants.AudioKoreanPkgVersion));
    }

    public GameAudioInstallation(bool chinese, bool english, bool japanese, bool korean)
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