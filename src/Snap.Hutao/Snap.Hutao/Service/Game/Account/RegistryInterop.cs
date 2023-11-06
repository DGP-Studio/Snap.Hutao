// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Win32;
using Snap.Hutao.Core.ExceptionService;
using Snap.Hutao.Model.Entity;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace Snap.Hutao.Service.Game.Account;

/// <summary>
/// 注册表操作
/// </summary>
internal static class RegistryInterop
{
    private const string GenshinPath = @"Software\miHoYo\原神";
    private const string GenshinKey = $@"HKEY_CURRENT_USER\{GenshinPath}";
    private const string SdkChineseKey = "MIHOYOSDK_ADL_PROD_CN_h3123967166";

    /// <summary>
    /// 设置键值
    /// 需要支持
    /// https://learn.microsoft.com/zh-cn/windows/win32/fileio/maximum-file-path-limitation
    /// </summary>
    /// <param name="account">账户</param>
    /// <param name="powerShellPath">PowerShell 路径</param>
    /// <returns>账号是否设置</returns>
    public static bool Set(GameAccount? account, string powerShellPath)
    {
        if (account is not null)
        {
            // 存回注册表的字节需要 '\0' 结尾
            Encoding.UTF8.GetByteCount(account.MihoyoSDK);
            byte[] tempBytes = Encoding.UTF8.GetBytes(account.MihoyoSDK);
            byte[] target = new byte[tempBytes.Length + 1];
            tempBytes.CopyTo(target, 0);

            string base64 = Convert.ToBase64String(target);
            string path = $"HKCU:{GenshinPath}";
            string command = $"""
                $value = [Convert]::FromBase64String('{base64}');
                Set-ItemProperty -Path '{path}' -Name '{SdkChineseKey}' -Value $value -Force;
                """;

            ProcessStartInfo startInfo = new()
            {
                Arguments = command,
                WorkingDirectory = Path.GetDirectoryName(powerShellPath),
                CreateNoWindow = true,
                FileName = powerShellPath,
            };

            try
            {
                System.Diagnostics.Process.Start(startInfo)?.WaitForExit();
            }
            catch (Win32Exception ex)
            {
                ThrowHelper.RuntimeEnvironment(SH.ServiceGameRegisteryInteropLongPathsDisabled, ex);
            }

            if (Get() == account.MihoyoSDK)
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// 在注册表中获取账号信息
    /// </summary>
    /// <returns>当前注册表中的信息</returns>
    public static unsafe string? Get()
    {
        object? sdk = Registry.GetValue(GenshinKey, SdkChineseKey, Array.Empty<byte>());

        if (sdk is byte[] bytes)
        {
            fixed (byte* pByte = bytes)
            {
                // 从注册表获取的字节数组带有 '\0' 结尾，需要舍去
                ReadOnlySpan<byte> span = MemoryMarshal.CreateReadOnlySpanFromNullTerminated(pByte);
                return Encoding.UTF8.GetString(span);
            }
        }

        return null;
    }
}