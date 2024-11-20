// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Win32;
using Snap.Hutao.Core.IO.Ini;
using System.IO;
using System.Text.RegularExpressions;

namespace Snap.Hutao.Service.Game.Locator;

[ConstructorGenerated]
[Injection(InjectAs.Transient, typeof(IGameLocator), Key = GameLocationSource.Registry)]
internal sealed partial class RegistryLauncherLocator : IGameLocator
{
    private const string RegistryKeyName = @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\原神";
    private readonly ITaskContext taskContext;

    [GeneratedRegex(@"\\x(?=[0-9a-f]{4})")]
    private static partial Regex UTF16Regex { get; }

    public async ValueTask<ValueResult<bool, string>> LocateGamePathAsync()
    {
        await taskContext.SwitchToBackgroundAsync();

        ValueResult<bool, string> result = LocateInternal("DisplayIcon");

        if (result.IsOk == false)
        {
            return result;
        }

        string? path = Path.GetDirectoryName(result.Value);
        ArgumentException.ThrowIfNullOrEmpty(path);
        string configPath = Path.Combine(path, GameConstants.ConfigFileName);

        string? escapedPath;
        using (FileStream stream = File.OpenRead(configPath))
        {
            IEnumerable<IniElement> elements = IniSerializer.Deserialize(stream);
            escapedPath = elements
                .OfType<IniParameter>()
                .FirstOrDefault(p => p.Key == "game_install_path")?.Value;
        }

        if (!string.IsNullOrEmpty(escapedPath))
        {
            string gamePath = Path.Combine(Unescape(escapedPath), GameConstants.YuanShenFileName);
            return new(true, gamePath);
        }

        return new(false, string.Empty);
    }

    private static ValueResult<bool, string> LocateInternal(string valueName)
    {
        if (Registry.GetValue(RegistryKeyName, valueName, null) is string path)
        {
            return new(true, path);
        }

        return new(false, default!);
    }

    private static string Unescape(string str)
    {
        string hex4Result = UTF16Regex.Replace(str, @"\u");

        // 不包含中文
        // Some one's folder might begin with 'u'
        if (!hex4Result.Contains(@"\u", StringComparison.Ordinal))
        {
            // fix path with \
            hex4Result = hex4Result.Replace(@"\", @"\\", StringComparison.Ordinal);
        }

        return Regex.Unescape(hex4Result);
    }
}