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
[HighQuality]
[Injection(InjectAs.Transient, typeof(IGameLocator))]
internal sealed partial class RegistryLauncherLocator : IGameLocator
{
    private readonly ITaskContext taskContext;

    /// <summary>
    /// 构造一个新的注册表启动器位置定位器
    /// </summary>
    /// <param name="taskContext">任务上下文</param>
    public RegistryLauncherLocator(ITaskContext taskContext)
    {
        this.taskContext = taskContext;
    }

    /// <inheritdoc/>
    public string Name { get => nameof(RegistryLauncherLocator); }

    /// <inheritdoc/>
    public async Task<ValueResult<bool, string>> LocateGamePathAsync()
    {
        await taskContext.SwitchToBackgroundAsync();

        ValueResult<bool, string> result = LocateInternal("DisplayIcon");

        if (result.IsOk == false)
        {
            return result;
        }
        else
        {
            string? path = Path.GetDirectoryName(result.Value);
            string configPath = Path.Combine(path!, GameConstants.ConfigFileName);
            string? escapedPath = null;
            using (FileStream stream = File.OpenRead(configPath))
            {
                IEnumerable<IniElement> elements = IniSerializer.Deserialize(stream);
                escapedPath = elements
                    .OfType<IniParameter>()
                    .FirstOrDefault(p => p.Key == "game_install_path")?.Value;
            }

            if (escapedPath != null)
            {
                string gamePath = Path.Combine(Unescape(escapedPath), GameConstants.YuanShenFileName);
                return new(true, gamePath);
            }
        }

        return new(false, string.Empty);
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
        // Some one's folder might begin with 'u'
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