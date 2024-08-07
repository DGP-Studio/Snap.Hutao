// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.IO.Ini;
using Snap.Hutao.Service.Game.Scheme;
using System.IO;

namespace Snap.Hutao.Service.Game;

internal static class GameFileSystemExtension
{
    public static void ExtractConfigurationFile(this GameFileSystem gameFileSystem, string version, LaunchScheme launchScheme)
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

        string? directory = Path.GetDirectoryName(gameFileSystem.GameConfigFilePath);
        ArgumentNullException.ThrowIfNull(directory);
        Directory.CreateDirectory(directory);
        File.WriteAllText(gameFileSystem.GameConfigFilePath, content);
    }

    public static bool TryFixConfigurationFile(this GameFileSystem gameFileSystem, LaunchScheme launchScheme)
    {
        if (!File.Exists(gameFileSystem.ScriptVersionFilePath))
        {
            return false;
        }

        string version = File.ReadAllText(gameFileSystem.ScriptVersionFilePath);
        ExtractConfigurationFile(gameFileSystem, version, launchScheme);

        return true;
    }

    public static bool TryFixScriptVersion(this GameFileSystem gameFileSystem)
    {
        if (!File.Exists(gameFileSystem.GameConfigFilePath))
        {
            return false;
        }

        List<IniParameter> parameters = IniSerializer.DeserializeFromFile(gameFileSystem.GameConfigFilePath).OfType<IniParameter>().ToList();

        string? version = parameters.FirstOrDefault(p => p.Key == "game_version")?.Value;

        string? directory = Path.GetDirectoryName(gameFileSystem.ScriptVersionFilePath);
        ArgumentNullException.ThrowIfNull(directory);
        Directory.CreateDirectory(directory);
        File.WriteAllText(gameFileSystem.ScriptVersionFilePath, version);
        return true;
    }
}