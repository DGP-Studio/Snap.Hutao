// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Service.Game.Scheme;
using System.IO;

namespace Snap.Hutao.Service.Game;

internal sealed partial class GameFileSystem : IGameFileSystem
{
    private readonly AsyncReaderWriterLock.Releaser releaser;

    public GameFileSystem(string gameFilePath, AsyncReaderWriterLock.Releaser releaser)
    {
        GameFilePath = gameFilePath;
        this.releaser = releaser;
    }

    public string GameFilePath { get; }

    [field: MaybeNull]
    public string GameFileName { get => field ??= Path.GetFileName(GameFilePath); }

    [field: MaybeNull]
    public string GameDirectory
    {
        get
        {
            if (field is not null)
            {
                return field;
            }

            string? directoryName = Path.GetDirectoryName(GameFilePath);
            ArgumentException.ThrowIfNullOrEmpty(directoryName);
            return field = directoryName;
        }
    }

    [field: MaybeNull]
    public string GameConfigFilePath { get => field ??= Path.Combine(GameDirectory, GameConstants.ConfigFileName); }

    [field: MaybeNull]
    public string PCGameSDKFilePath { get => field ??= Path.Combine(GameDirectory, GameConstants.PCGameSDKFilePath); }

    [field: MaybeNull]
    public string ScreenShotDirectory { get => field ??= Path.Combine(GameDirectory, "ScreenShot"); }

    [field: MaybeNull]
    public string DataDirectory { get => field ??= Path.Combine(GameDirectory, LaunchScheme.ExecutableIsOversea(GameFileName) ? GameConstants.GenshinImpactData : GameConstants.YuanShenData); }

    [field: MaybeNull]
    public string ScriptVersionFilePath { get => field ??= Path.Combine(DataDirectory, "Persistent", "ScriptVersion"); }

    [field: MaybeNull]
    public string ChunksDirectory { get => field ??= Path.Combine(GameDirectory, "chunks"); }

    [field: MaybeNull]
    public string PredownloadStatusPath { get => field ??= Path.Combine(ChunksDirectory, "snap_hutao_predownload_status.json"); }

    [field: MaybeNull]
    public GameAudioSystem Audio { get => field ??= new(GameFilePath); }

    public void Dispose()
    {
        releaser.Dispose();
    }
}