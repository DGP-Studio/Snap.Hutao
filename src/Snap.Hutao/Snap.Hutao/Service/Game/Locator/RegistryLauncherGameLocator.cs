// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Win32;
using Snap.Hutao.Core.IO.Ini;
using System.Collections.Immutable;
using System.IO;
using System.Text.RegularExpressions;

namespace Snap.Hutao.Service.Game.Locator;

[ConstructorGenerated]
[Injection(InjectAs.Transient, typeof(IGameLocator), Key = GameLocationSourceKind.Registry)]
internal sealed partial class RegistryLauncherGameLocator : IGameLocator, IGameLocator2
{
    private const string RegistryKeyNameCn = @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\原神";
    private const string RegistryKeyNameOs = @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\Genshin Impact";

    private readonly ITaskContext taskContext;

    [GeneratedRegex(@"\\x(?=[0-9a-f]{4})")]
    private static partial Regex Utf16Regex { get; }

    public async ValueTask<ValueResult<bool, string>> LocateSingleGamePathAsync()
    {
        ValueResult<bool, string> osResult = await LocateGamePathAsync(RegistryKeyNameOs, GameConstants.GenshinImpactFileName).ConfigureAwait(false);
        if (osResult.IsOk)
        {
            return osResult;
        }

        return await LocateGamePathAsync(RegistryKeyNameCn, GameConstants.YuanShenFileName).ConfigureAwait(false);
    }

    public async ValueTask<ImmutableArray<string>> LocateMultipleGamePathAsync()
    {
        ValueResult<bool, string> osResult = await LocateGamePathAsync(RegistryKeyNameOs, GameConstants.GenshinImpactFileName).ConfigureAwait(false);
        ValueResult<bool, string> cnResult = await LocateGamePathAsync(RegistryKeyNameCn, GameConstants.YuanShenFileName).ConfigureAwait(false);
        ImmutableArray<string>.Builder builder = ImmutableArray.CreateBuilder<string>(2);
        if (osResult.IsOk)
        {
            builder.Add(osResult.Value);
        }

        if (cnResult.IsOk)
        {
            builder.Add(cnResult.Value);
        }

        return builder.ToImmutable();
    }

    private static ValueResult<bool, string> LocateLauncher(string registryKey, string valueName)
    {
        if (Registry.GetValue(registryKey, valueName, null) is string path)
        {
            return new(true, path);
        }

        return new(false, default!);
    }

    private static string Unescape(string str)
    {
        string hex4Result = Utf16Regex.Replace(str, @"\u");

        // 不包含中文
        // Someone's folder might begin with 'u'
        if (!hex4Result.Contains(@"\u", StringComparison.Ordinal))
        {
            // Fix path with \
            hex4Result = hex4Result.Replace(@"\", @"\\", StringComparison.Ordinal);
        }

        return Regex.Unescape(hex4Result);
    }

    private async ValueTask<ValueResult<bool, string>> LocateGamePathAsync(string registryKey, string executableName)
    {
        await taskContext.SwitchToBackgroundAsync();

        ValueResult<bool, string> result = LocateLauncher(registryKey, "DisplayIcon");

        if (!result.IsOk)
        {
            return result;
        }

        string? path = Path.GetDirectoryName(result.Value);
        ArgumentException.ThrowIfNullOrEmpty(path);
        string configPath = Path.Combine(path, GameConstants.ConfigFileName);

        string? escapedPath;
        using (FileStream stream = File.OpenRead(configPath))
        {
            escapedPath = IniSerializer.Deserialize(stream)
                .OfType<IniParameter>()
                .FirstOrDefault(p => p.Key == "game_install_path")?.Value;
        }

        if (!string.IsNullOrEmpty(escapedPath))
        {
            string gamePath = Path.Combine(Unescape(escapedPath), executableName);
            return new(true, gamePath);
        }

        return new(false, string.Empty);
    }
}