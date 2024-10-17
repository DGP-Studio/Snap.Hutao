// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.IO;

namespace Snap.Hutao.Service.Game;

internal sealed class GameAudioSystem
{
    private readonly string gameDirectory;

    private bool? chinese;
    private bool? english;
    private bool? japanese;
    private bool? korean;

    public GameAudioSystem(string gameFilePath)
    {
        string? directory = Path.GetDirectoryName(gameFilePath);
        ArgumentException.ThrowIfNullOrEmpty(directory);
        gameDirectory = directory;
    }

    public GameAudioSystem(bool chinese, bool english, bool japanese, bool korean)
    {
        gameDirectory = default!;

        this.chinese = chinese;
        this.english = english;
        this.japanese = japanese;
        this.korean = korean;
    }

    public bool Chinese { get => chinese ??= File.Exists(Path.Combine(gameDirectory, GameConstants.AudioChinesePkgVersion)); }

    public bool English { get => english ??= File.Exists(Path.Combine(gameDirectory, GameConstants.AudioEnglishPkgVersion)); }

    public bool Japanese { get => japanese ??= File.Exists(Path.Combine(gameDirectory, GameConstants.AudioJapanesePkgVersion)); }

    public bool Korean { get => korean ??= File.Exists(Path.Combine(gameDirectory, GameConstants.AudioKoreanPkgVersion)); }
}
