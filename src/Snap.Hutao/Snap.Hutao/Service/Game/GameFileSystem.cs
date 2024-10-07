// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Service.Game.Scheme;
using System.IO;

namespace Snap.Hutao.Service.Game;

internal sealed class GameFileSystem
{
    private readonly string gameFilePath;

    private string? gameFileName;
    private string? gameDirectory;
    private string? gameConfigFilePath;
    private string? pcGameSDKFilePath;
    private string? extractDirectory;
    private GameAudioSystem? gameAudioSystem;

    public GameFileSystem(string gameFilePath)
    {
        this.gameFilePath = gameFilePath;
    }

    public GameFileSystem(string gameFilePath, GameAudioSystem gameAudioSystem)
    {
        this.gameFilePath = gameFilePath;
        this.gameAudioSystem = gameAudioSystem;
    }

    public string GameFilePath { get => gameFilePath; }

    public string GameFileName { get => gameFileName ??= Path.GetFileName(gameFilePath); }

    public string GameDirectory
    {
        get
        {
            gameDirectory ??= Path.GetDirectoryName(gameFilePath);
            ArgumentException.ThrowIfNullOrEmpty(gameDirectory);
            return gameDirectory;
        }
    }

    public string GameConfigFilePath { get => gameConfigFilePath ??= Path.Combine(GameDirectory, GameConstants.ConfigFileName); }

    public string PCGameSDKFilePath { get => pcGameSDKFilePath ??= Path.Combine(GameDirectory, GameConstants.PCGameSDKFilePath); }

    public string ScreenShotDirectory { get => Path.Combine(GameDirectory, "ScreenShot"); }

    public string DataDirectory { get => Path.Combine(GameDirectory, LaunchScheme.ExecutableIsOversea(GameFileName) ? GameConstants.GenshinImpactData : GameConstants.YuanShenData); }

    public string ScriptVersionFilePath { get => Path.Combine(DataDirectory, "Persistent", "ScriptVersion"); }

    public string ChunksDirectory { get => Path.Combine(GameDirectory, "chunks"); }

    public string PredownloadStatusPath { get => Path.Combine(ChunksDirectory, "snap_hutao_predownload_status.json"); }

    public string ExtractDirectory { get => extractDirectory ??= GameDirectory; set => extractDirectory = value; }

    public GameAudioSystem GameAudioSystem { get => gameAudioSystem ??= new(GameFilePath); }
}