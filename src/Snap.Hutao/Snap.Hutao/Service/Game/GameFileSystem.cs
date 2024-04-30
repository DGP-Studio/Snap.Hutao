// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.IO;

namespace Snap.Hutao.Service.Game;

internal sealed class GameFileSystem
{
    private readonly string gameFilePath;

    private string? gameFileName;
    private string? gameDirectory;
    private string? gameConfigFilePath;
    private string? pcGameSDKFilePath;

    public GameFileSystem(string gameFilePath)
    {
        this.gameFilePath = gameFilePath;
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
}