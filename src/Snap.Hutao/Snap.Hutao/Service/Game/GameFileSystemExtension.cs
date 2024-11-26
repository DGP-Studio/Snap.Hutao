// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.ExceptionService;
using Snap.Hutao.Core.IO.Ini;
using Snap.Hutao.Service.Game.Scheme;
using System.Collections.Immutable;
using System.IO;
using System.Runtime.CompilerServices;

namespace Snap.Hutao.Service.Game;

internal static class GameFileSystemExtension
{
    private static readonly ConditionalWeakTable<IGameFileSystem, string> GameFileSystemGameFileNames = [];
    private static readonly ConditionalWeakTable<IGameFileSystem, string> GameFileSystemGameDirectories = [];
    private static readonly ConditionalWeakTable<IGameFileSystem, string> GameFileSystemGameConfigurationFilePaths = [];
    private static readonly ConditionalWeakTable<IGameFileSystem, string> GameFileSystemPcGameSdkFilePaths = [];
    private static readonly ConditionalWeakTable<IGameFileSystem, string> GameFileSystemScreenShotDirectories = [];
    private static readonly ConditionalWeakTable<IGameFileSystem, string> GameFileSystemDataDirectories = [];
    private static readonly ConditionalWeakTable<IGameFileSystem, string> GameFileSystemScriptVersionFilePaths = [];
    private static readonly ConditionalWeakTable<IGameFileSystem, string> GameFileSystemChunksDirectories = [];
    private static readonly ConditionalWeakTable<IGameFileSystem, string> GameFileSystemPredownloadStatusPaths = [];

    public static string GetGameFileName(this IGameFileSystem gameFileSystem)
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

    public static string GetGameDirectory(this IGameFileSystem gameFileSystem)
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

    public static string GetGameConfigurationFilePath(this IGameFileSystem gameFileSystem)
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

    public static string GetPcGameSdkFilePath(this IGameFileSystem gameFileSystem)
    {
        ObjectDisposedException.ThrowIf(gameFileSystem.IsDisposed, gameFileSystem);

        // ReSharper disable once InconsistentNaming
        if (GameFileSystemPcGameSdkFilePaths.TryGetValue(gameFileSystem, out string? pcGameSdkFilePath))
        {
            return pcGameSdkFilePath;
        }

        pcGameSdkFilePath = string.Intern(Path.Combine(gameFileSystem.GetGameDirectory(), GameConstants.PCGameSDKFilePath));
        GameFileSystemPcGameSdkFilePaths.Add(gameFileSystem, pcGameSdkFilePath);
        return pcGameSdkFilePath;
    }

    public static string GetScreenShotDirectory(this IGameFileSystem gameFileSystem)
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

    public static string GetDataDirectory(this IGameFileSystem gameFileSystem)
    {
        ObjectDisposedException.ThrowIf(gameFileSystem.IsDisposed, gameFileSystem);

        if (GameFileSystemDataDirectories.TryGetValue(gameFileSystem, out string? dataDirectory))
        {
            return dataDirectory;
        }

        string dataDirectoryName = gameFileSystem.IsOversea() ? GameConstants.GenshinImpactData : GameConstants.YuanShenData;
        dataDirectory = string.Intern(Path.Combine(gameFileSystem.GetGameDirectory(), dataDirectoryName));
        GameFileSystemDataDirectories.Add(gameFileSystem, dataDirectory);
        return dataDirectory;
    }

    public static string GetScriptVersionFilePath(this IGameFileSystem gameFileSystem)
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

    public static string GetChunksDirectory(this IGameFileSystem gameFileSystem)
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

    public static string GetPredownloadStatusPath(this IGameFileSystem gameFileSystem)
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

    public static bool IsOversea(this IGameFileSystem gameFileSystem)
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

    public static bool TryGetGameVersion(this IGameFileSystem gameFileSystem, [NotNullWhen(true)] out string? version)
    {
        version = default!;
        if (File.Exists(gameFileSystem.GetGameConfigurationFilePath()))
        {
            foreach (ref readonly IniElement element in IniSerializer.DeserializeFromFile(gameFileSystem.GetGameConfigurationFilePath()).AsSpan())
            {
                if (element is IniParameter { Key: "game_version" } parameter)
                {
                    version = parameter.Value;
                    break;
                }
            }

            return true;
        }

        if (File.Exists(gameFileSystem.GetScriptVersionFilePath()))
        {
            version = File.ReadAllText(gameFileSystem.GetScriptVersionFilePath());
            return true;
        }

        return false;
    }

    public static void GenerateConfigurationFile(this IGameFileSystem gameFileSystem, string version, LaunchScheme launchScheme)
    {
        string gameBiz = launchScheme.IsOversea ? "hk4e_global" : "hk4e_cn";
        string content = $$$"""
            [General]
            channel={{{launchScheme.Channel:D}}}
            cps=mihoyo
            game_version={{{version}}}
            sub_channel={{{launchScheme.SubChannel:D}}}
            sdk_version=
            game_biz={{{gameBiz}}}
            uapc={"hk4e_cn":{"uapc":""},"hyp":{"uapc":""}}
            """;

        string? directory = Path.GetDirectoryName(gameFileSystem.GetGameConfigurationFilePath());
        ArgumentNullException.ThrowIfNull(directory);
        Directory.CreateDirectory(directory);
        File.WriteAllText(gameFileSystem.GetGameConfigurationFilePath(), content);
    }

    public static bool TryUpdateConfigurationFile(this IGameFileSystem gameFileSystem, string version)
    {
        bool updated = false;
        ImmutableArray<IniElement> ini = IniSerializer.DeserializeFromFile(gameFileSystem.GetGameConfigurationFilePath());
        foreach (ref readonly IniElement element in ini.AsSpan())
        {
            if (element is not IniParameter { Key: "game_version" } parameter)
            {
                continue;
            }

            updated = parameter.Set(version);
        }

        IniSerializer.SerializeToFile(gameFileSystem.GetGameConfigurationFilePath(), ini);
        return updated;
    }

    public static bool TryFixConfigurationFile(this IGameFileSystem gameFileSystem, LaunchScheme launchScheme)
    {
        if (!File.Exists(gameFileSystem.GetScriptVersionFilePath()))
        {
            return false;
        }

        string version = File.ReadAllText(gameFileSystem.GetScriptVersionFilePath());
        GenerateConfigurationFile(gameFileSystem, version, launchScheme);

        return true;
    }

    public static bool TryFixScriptVersion(this IGameFileSystem gameFileSystem)
    {
        if (!File.Exists(gameFileSystem.GetGameConfigurationFilePath()))
        {
            return false;
        }

        string? version = default;
        foreach (ref readonly IniElement element in IniSerializer.DeserializeFromFile(gameFileSystem.GetGameConfigurationFilePath()).AsSpan())
        {
            if (element is IniParameter { Key: "game_version" } parameter)
            {
                version = parameter.Value;
                break;
            }
        }

        string? directory = Path.GetDirectoryName(gameFileSystem.GetScriptVersionFilePath());
        ArgumentNullException.ThrowIfNull(directory);

        Directory.CreateDirectory(directory);
        File.WriteAllText(gameFileSystem.GetScriptVersionFilePath(), version);
        return true;
    }
}