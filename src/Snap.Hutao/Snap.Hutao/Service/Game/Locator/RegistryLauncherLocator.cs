// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Win32;
using Snap.Hutao.Core.IO.Ini;
using System.IO;
using System.Text.RegularExpressions;

namespace Snap.Hutao.Service.Game.Locator;

/// <summary>
/// 注册表启动器位置定位器
/// </summary>
[Injection(InjectAs.Transient, typeof(IGameLocator))]
internal partial class RegistryLauncherLocator : IGameLocator
{
    /// <inheritdoc/>
    public string Name { get => nameof(RegistryLauncherLocator); }

    /// <inheritdoc/>
    public Task<ValueResult<bool, string>> LocateGamePathAsync()
    {
        ValueResult<bool, string> result = LocateInternal("DisplayIcon");

        if (result.IsOk == false)
        {
            return Task.FromResult(result);
        }
        else
        {
            string? path = Path.GetDirectoryName(result.Value);
            string configPath = Path.Combine(path!, "config.ini");
            string? escapedPath = null;
            using (FileStream stream = File.OpenRead(configPath))
            {
                IEnumerable<IniElement> elements = IniSerializer.Deserialize(stream);
                escapedPath = elements.OfType<IniParameter>().FirstOrDefault(p => p.Key == "game_install_path")?.Value;
            }

            if (escapedPath != null)
            {
                string gamePath = Path.Combine(Unescape(escapedPath), "YuanShen.exe");
                return Task.FromResult<ValueResult<bool, string>>(new(true, gamePath));
            }
        }

        return Task.FromResult<ValueResult<bool, string>>(new(false, null!));
    }

    private static ValueResult<bool, string> LocateInternal(string key)
    {
        using (RegistryKey? uninstallKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\原神"))
        {
            if (uninstallKey != null)
            {
                if (uninstallKey.GetValue(key) is string path)
                {
                    return new(true, path);
                }
                else
                {
                    return new(false, null!);
                }
            }
            else
            {
                return new(false, null!);
            }
        }
    }

    private static string Unescape(string str)
    {
        string? hex4Result = UTF16Regex().Replace(str, @"\u$1");

        // 不包含中文
        if (!hex4Result.Contains(@"\u"))
        {
            // fix path with \
            hex4Result = hex4Result.Replace(@"\", @"\\");
        }

        return Regex.Unescape(hex4Result);
    }

    [GeneratedRegex("\\\\x([0-9a-f]{4})")]
    private static partial Regex UTF16Regex();
}