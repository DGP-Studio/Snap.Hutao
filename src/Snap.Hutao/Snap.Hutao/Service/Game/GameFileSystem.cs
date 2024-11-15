// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Service.Game.Scheme;
using System.IO;

namespace Snap.Hutao.Service.Game;

internal sealed class GameFileSystem
{
    public GameFileSystem(string gameFilePath)
    {
        GameFilePath = gameFilePath;
    }

    public GameFileSystem(string gameFilePath, GameAudioSystem gameAudioSystem)
    {
        GameFilePath = gameFilePath;
        Audio = gameAudioSystem;
    }

    public string GameFilePath { get; }

    [field: MaybeNull]
    public string GameFileName { get => field ??= Path.GetFileName(GameFilePath); }

    [field: AllowNull]
    [field: MaybeNull]
    public string GameDirectory
    {
        get
        {
            field ??= Path.GetDirectoryName(GameFilePath);
            ArgumentException.ThrowIfNullOrEmpty(field);
            return field;
        }
    }

    [field: MaybeNull]
    public string GameConfigFilePath { get => field ??= Path.Combine(GameDirectory, GameConstants.ConfigFileName); }

    [field: MaybeNull]
    public string PCGameSDKFilePath { get => field ??= Path.Combine(GameDirectory, GameConstants.PCGameSDKFilePath); }

    public string ScreenShotDirectory { get => Path.Combine(GameDirectory, "ScreenShot"); }

    public string DataDirectory { get => Path.Combine(GameDirectory, LaunchScheme.ExecutableIsOversea(GameFileName) ? GameConstants.GenshinImpactData : GameConstants.YuanShenData); }

    public string ScriptVersionFilePath { get => Path.Combine(DataDirectory, "Persistent", "ScriptVersion"); }

    public string ChunksDirectory { get => Path.Combine(GameDirectory, "chunks"); }

    public string PredownloadStatusPath { get => Path.Combine(ChunksDirectory, "snap_hutao_predownload_status.json"); }

    [field: MaybeNull]
    public GameAudioSystem Audio { get => field ??= new(GameFilePath); }
}