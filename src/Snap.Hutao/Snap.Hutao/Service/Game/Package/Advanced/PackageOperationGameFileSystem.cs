// // Copyright (c) DGP Studio. All rights reserved.
// // Licensed under the MIT license.

using Snap.Hutao.Service.Game.Scheme;
using System.IO;

namespace Snap.Hutao.Service.Game.Package.Advanced;

internal sealed partial class PackageOperationGameFileSystem : IGameFileSystem
{
    public PackageOperationGameFileSystem(string gameFilePath)
    {
        GameFilePath = gameFilePath;
    }

    public PackageOperationGameFileSystem(string gameFilePath, GameAudioSystem gameAudioSystem)
    {
        GameFilePath = gameFilePath;
        Audio = gameAudioSystem;
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

    // ReSharper disable once InconsistentNaming
    [field: MaybeNull]
    public string PCGameSDKFilePath { get => field ??= Path.Combine(GameDirectory, GameConstants.PCGameSDKFilePath); }

    public string ScreenShotDirectory { get => Path.Combine(GameDirectory, "ScreenShot"); }

    public string DataDirectory { get => Path.Combine(GameDirectory, LaunchScheme.ExecutableIsOversea(GameFileName) ? GameConstants.GenshinImpactData : GameConstants.YuanShenData); }

    public string ScriptVersionFilePath { get => Path.Combine(DataDirectory, "Persistent", "ScriptVersion"); }

    public string ChunksDirectory { get => Path.Combine(GameDirectory, "chunks"); }

    public string PredownloadStatusPath { get => Path.Combine(ChunksDirectory, "snap_hutao_predownload_status.json"); }

    [field: MaybeNull]
    public GameAudioSystem Audio { get => field ??= new(GameFilePath); }

    public void Dispose()
    {
    }
}