// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Win32;
using Snap.Hutao.Core.Threading;

namespace Snap.Hutao.Service.Game.Locator;

/// <summary>
/// 注册表启动器位置定位器
/// </summary>
[Injection(InjectAs.Transient, typeof(IGameLocator))]
internal class RegistryLauncherLocator : IGameLocator
{
    /// <inheritdoc/>
    public string Name { get => nameof(RegistryLauncherLocator); }

    /// <inheritdoc/>
    public Task<ValueResult<bool, string>> LocateGamePathAsync()
    {
        return Task.FromResult(LocateInternal("InstallPath", "\\Genshin Impact Game\\YuanShen.exe"));
    }

    /// <inheritdoc/>
    public Task<ValueResult<bool, string>> LocateLauncherPathAsync()
    {
        return Task.FromResult(LocateInternal("DisplayIcon"));
    }

    private static ValueResult<bool, string> LocateInternal(string key, string? append = null)
    {
        RegistryKey? uninstallKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\原神");
        if (uninstallKey != null)
        {
            if (uninstallKey.GetValue(key) is string path)
            {
                if (!string.IsNullOrEmpty(append))
                {
                    path += append;
                }

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