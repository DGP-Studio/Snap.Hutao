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

    public static bool Set(GameAccount? account)
    {
        if (account is not null)
        {
            // 存回注册表的字节需要 '\0' 结尾
            byte[] target = [.. Encoding.UTF8.GetBytes(account.MihoyoSDK), 0];
            Registry.SetValue(GenshinKey, SdkChineseKey, target);

            string? get = Get();
            if (get == account.MihoyoSDK)
            {
                return true;
            }
        }

        return false;
    }

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