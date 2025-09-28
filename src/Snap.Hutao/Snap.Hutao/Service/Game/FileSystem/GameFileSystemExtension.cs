// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.ExceptionService;
using Snap.Hutao.Core.IO.Ini;
using System.IO;
using System.Runtime.CompilerServices;

namespace Snap.Hutao.Service.Game.FileSystem;

internal static class GameFileSystemExtension
{
    private static readonly ConditionalWeakTable<IGameFileSystemView, string> GameFileSystemGameFileNames = [];
    private static readonly ConditionalWeakTable<IGameFileSystemView, string> GameFileSystemGameDirectories = [];
    private static readonly ConditionalWeakTable<IGameFileSystemView, string> GameFileSystemGameConfigurationFilePaths = [];
    private static readonly ConditionalWeakTable<IGameFileSystemView, string> GameFileSystemPcGameSdkFilePaths = [];
    private static readonly ConditionalWeakTable<IGameFileSystemView, string> GameFileSystemScreenShotDirectories = [];
    private static readonly ConditionalWeakTable<IGameFileSystemView, string> GameFileSystemDataDirectories = [];
    private static readonly ConditionalWeakTable<IGameFileSystemView, string> GameFileSystemScriptVersionFilePaths = [];
    private static readonly ConditionalWeakTable<IGameFileSystemView, string> GameFileSystemChunksDirectories = [];
    private static readonly ConditionalWeakTable<IGameFileSystemView, string> GameFileSystemPredownloadStatusPaths = [];

    public static string GetGameFileName(this IGameFileSystemView gameFileSystem)
    {
        ObjectDisposedException.ThrowIf(gameFileSystem.IsDisposed, gameFileSystem);

        if (GameFileSystemGameFileNames.TryGetValue(gameFileSystem, out string? gameFileName))
        {
            return gameFileName;
        }

        gameFileName = string.Intern(Path.GetFileName(gameFileSystem.GameFilePath));
        GameFileSystemGameFileNames.Add(gameFileSystem, gameFileName);
        return gameFileName;
    }

    public static string GetGameDirectory(this IGameFileSystemView gameFileSystem)
    {
        ObjectDisposedException.ThrowIf(gameFileSystem.IsDisposed, gameFileSystem);

        if (GameFileSystemGameDirectories.TryGetValue(gameFileSystem, out string? gameDirectory))
        {
            return gameDirectory;
        }

        gameDirectory = Path.GetDirectoryName(gameFileSystem.GameFilePath);
        ArgumentException.ThrowIfNullOrEmpty(gameDirectory);
        string internedGameDirectory = string.Intern(gameDirectory);
        GameFileSystemGameDirectories.Add(gameFileSystem, internedGameDirectory);
        return internedGameDirectory;
    }

    public static string GetGameConfigurationFilePath(this IGameFileSystemView gameFileSystem)
    {
        ObjectDisposedException.ThrowIf(gameFileSystem.IsDisposed, gameFileSystem);

        if (GameFileSystemGameConfigurationFilePaths.TryGetValue(gameFileSystem, out string? gameConfigFilePath))
        {
            return gameConfigFilePath;
        }

        gameConfigFilePath = string.Intern(Path.Combine(gameFileSystem.GetGameDirectory(), GameConstants.ConfigFileName));
        GameFileSystemGameConfigurationFilePaths.Add(gameFileSystem, gameConfigFilePath);
        return gameConfigFilePath;
    }

    public static string GetPCGameSDKFilePath(this IGameFileSystemView gameFileSystem)
    {
        ObjectDisposedException.ThrowIf(gameFileSystem.IsDisposed, gameFileSystem);

        if (GameFileSystemPcGameSdkFilePaths.TryGetValue(gameFileSystem, out string? pcGameSdkFilePath))
        {
            return pcGameSdkFilePath;
        }

        pcGameSdkFilePath = string.Intern(Path.Combine(gameFileSystem.GetGameDirectory(), GameConstants.PCGameSDKFilePath));
        GameFileSystemPcGameSdkFilePaths.Add(gameFileSystem, pcGameSdkFilePath);
        return pcGameSdkFilePath;
    }

    public static string GetScreenShotDirectory(this IGameFileSystemView gameFileSystem)
    {
        ObjectDisposedException.ThrowIf(gameFileSystem.IsDisposed, gameFileSystem);

        if (GameFileSystemScreenShotDirectories.TryGetValue(gameFileSystem, out string? screenShotDirectory))
        {
            return screenShotDirectory;
        }

        screenShotDirectory = string.Intern(Path.Combine(gameFileSystem.GetGameDirectory(), "ScreenShot"));
        GameFileSystemScreenShotDirectories.Add(gameFileSystem, screenShotDirectory);
        return screenShotDirectory;
    }

    public static string GetDataDirectory(this IGameFileSystemView gameFileSystem)
    {
        ObjectDisposedException.ThrowIf(gameFileSystem.IsDisposed, gameFileSystem);

        if (GameFileSystemDataDirectories.TryGetValue(gameFileSystem, out string? dataDirectory))
        {
            return dataDirectory;
        }

        string dataDirectoryName = gameFileSystem.IsExecutableOversea() ? GameConstants.GenshinImpactData : GameConstants.YuanShenData;
        dataDirectory = string.Intern(Path.Combine(gameFileSystem.GetGameDirectory(), dataDirectoryName));
        GameFileSystemDataDirectories.Add(gameFileSystem, dataDirectory);
        return dataDirectory;
    }

    public static string GetScriptVersionFilePath(this IGameFileSystemView gameFileSystem)
    {
        ObjectDisposedException.ThrowIf(gameFileSystem.IsDisposed, gameFileSystem);

        if (GameFileSystemScriptVersionFilePaths.TryGetValue(gameFileSystem, out string? scriptVersionFilePath))
        {
            return scriptVersionFilePath;
        }

        scriptVersionFilePath = string.Intern(Path.Combine(gameFileSystem.GetDataDirectory(), "Persistent", "ScriptVersion"));
        GameFileSystemScriptVersionFilePaths.Add(gameFileSystem, scriptVersionFilePath);
        return scriptVersionFilePath;
    }

    public static string GetChunksDirectory(this IGameFileSystemView gameFileSystem)
    {
        ObjectDisposedException.ThrowIf(gameFileSystem.IsDisposed, gameFileSystem);

        if (GameFileSystemChunksDirectories.TryGetValue(gameFileSystem, out string? chunksDirectory))
        {
            return chunksDirectory;
        }

        chunksDirectory = string.Intern(Path.Combine(gameFileSystem.GetGameDirectory(), "chunks"));
        GameFileSystemChunksDirectories.Add(gameFileSystem, chunksDirectory);
        return chunksDirectory;
    }

    public static string GetPredownloadStatusFilePath(this IGameFileSystemView gameFileSystem)
    {
        ObjectDisposedException.ThrowIf(gameFileSystem.IsDisposed, gameFileSystem);

        if (GameFileSystemPredownloadStatusPaths.TryGetValue(gameFileSystem, out string? predownloadStatusPath))
        {
            return predownloadStatusPath;
        }

        predownloadStatusPath = string.Intern(Path.Combine(gameFileSystem.GetChunksDirectory(), "snap_hutao_predownload_status.json"));
        GameFileSystemPredownloadStatusPaths.Add(gameFileSystem, predownloadStatusPath);
        return predownloadStatusPath;
    }

    public static bool IsExecutableOversea(this IGameFileSystemView gameFileSystem)
    {
        ObjectDisposedException.ThrowIf(gameFileSystem.IsDisposed, gameFileSystem);

        string gameFileName = gameFileSystem.GetGameFileName();
        return gameFileName.ToUpperInvariant() switch
        {
            GameConstants.GenshinImpactFileNameUpper => true,
            GameConstants.YuanShenFileNameUpper => false,
            _ => throw HutaoException.Throw($"Invalid game executable file name：{gameFileName}"),
        };
    }

    public static bool TryGetGameVersion(this IGameFileSystemView gameFileSystem, [NotNullWhen(true)] out string? version)
    {
        version = default!;
        string configFilePath = gameFileSystem.GetGameConfigurationFilePath();
        if (File.Exists(configFilePath))
        {
            foreach (ref readonly IniElement element in IniSerializer.DeserializeFromFile(configFilePath).AsSpan())
            {
                if (element is IniParameter { Key: GameConstants.GameVersion, Value: { Length: > 0 } value })
                {
                    version = value;
                    return true;
                }
            }
        }

        string scriptVersionFilePath = gameFileSystem.GetScriptVersionFilePath();
        if (File.Exists(scriptVersionFilePath))
        {
            version = File.ReadAllText(scriptVersionFilePath);
            return true;
        }

        return false;
    }
}