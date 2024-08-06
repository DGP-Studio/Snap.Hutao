// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.IO.Ini;
using Snap.Hutao.Service.Game.Scheme;
using System.IO;

namespace Snap.Hutao.Service.Game;

internal static class GameFileSystemExtension
{
    public static async ValueTask ExtractConfigurationFileAsync(this GameFileSystem gameFileSystem, string version, LaunchScheme launchScheme)
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

        await File.WriteAllTextAsync(gameFileSystem.GameConfigFilePath, content).ConfigureAwait(false);
    }

    public static async ValueTask<bool> TryFixConfigurationFileAsync(this GameFileSystem gameFileSystem, LaunchScheme launchScheme)
    {
        if (!File.Exists(gameFileSystem.ScriptVersionFilePath))
        {
            return false;
        }

        string version = await File.ReadAllTextAsync(gameFileSystem.ScriptVersionFilePath).ConfigureAwait(false);
        await ExtractConfigurationFileAsync(gameFileSystem, version, launchScheme).ConfigureAwait(false);

        return true;
    }

    public static async ValueTask<bool> TryFixScriptVersionAsync(this GameFileSystem gameFileSystem)
    {
        if (!File.Exists(gameFileSystem.GameConfigFilePath))
        {
            return false;
        }

        List<IniParameter> parameters = IniSerializer.DeserializeFromFile(gameFileSystem.GameConfigFilePath).OfType<IniParameter>().ToList();

        string? version = parameters.FirstOrDefault(p => p.Key == "game_version")?.Value;

        await File.WriteAllTextAsync(gameFileSystem.ScriptVersionFilePath, version).ConfigureAwait(false);
        return true;
    }
}