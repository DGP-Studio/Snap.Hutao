// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core;
using Snap.Hutao.Core.IO;
using Snap.Hutao.Service.Game.Unlocker;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace Snap.Hutao.Service.Game;

/// <summary>
/// 进程互操作
/// </summary>
internal static class ProcessInterop
{
    /// <summary>
    /// 获取初始化后的游戏进程
    /// </summary>
    /// <param name="options">启动选项</param>
    /// <param name="gamePath">游戏路径</param>
    /// <returns>初始化后的游戏进程</returns>
    public static Process PrepareGameProcess(LaunchOptions options, string gamePath)
    {
        // https://docs.unity.cn/cn/current/Manual/PlayerCommandLineArguments.html
        string commandLine = new CommandLineBuilder()
            .AppendIf("-popupwindow", options.IsBorderless)
            .AppendIf("-window-mode", options.IsExclusive, "exclusive")
            .Append("-screen-fullscreen", options.IsFullScreen ? 1 : 0)
            .Append("-screen-width", options.ScreenWidth)
            .Append("-screen-height", options.ScreenHeight)
            .Append("-monitor", options.Monitor.Value)
            .ToString();

        return new()
        {
            StartInfo = new()
            {
                Arguments = commandLine,
                FileName = gamePath,
                UseShellExecute = true,
                Verb = "runas",
                WorkingDirectory = Path.GetDirectoryName(gamePath),
            },
        };
    }

    /// <summary>
    /// 解锁帧率
    /// </summary>
    /// <param name="game">游戏进程</param>
    /// <param name="options">启动选项</param>
    /// <returns>任务</returns>
    public static Task UnlockFpsAsync(Process game, LaunchOptions options)
    {
        IGameFpsUnlocker unlocker = new GameFpsUnlocker(game, options.TargetFps);

        TimeSpan findModuleDelay = TimeSpan.FromMilliseconds(100);
        TimeSpan findModuleLimit = TimeSpan.FromMilliseconds(10000);
        TimeSpan adjustFpsDelay = TimeSpan.FromMilliseconds(2000);

        return unlocker.UnlockAsync(findModuleDelay, findModuleLimit, adjustFpsDelay);
    }

    /// <summary>
    /// 尝试禁用mhypbase
    /// </summary>
    /// <param name="game">游戏进程</param>
    /// <param name="gamePath">游戏路径</param>
    /// <returns>是否禁用成功</returns>
    public static async Task<bool> DisableProtectionAsync(Process game, string gamePath)
    {
        string? gameFolder = Path.GetDirectoryName(gamePath);
        if (!string.IsNullOrEmpty(gameFolder))
        {
            string pbasePath = Path.Combine(gameFolder, "mhypbase.dll");
            SafeHandle handle = File.OpenHandle(pbasePath, share: FileShare.None);
            while (true) {
                if (game.Any(process => process.MainWindowHandle != nint.Zero)) {
                    handle.Close();
                    break;
                }
                await Task.Delay(100).ConfigureAwait(false);
            }
            return true;
        }

        return false;
    }
}