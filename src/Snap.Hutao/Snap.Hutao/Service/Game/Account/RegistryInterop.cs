// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Win32;
using Snap.Hutao.Core.ExceptionService;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Model.Entity.Primitive;
using System.Runtime.InteropServices;
using System.Text;

namespace Snap.Hutao.Service.Game.Account;

/// <summary>
/// 注册表操作
/// </summary>
internal static class RegistryInterop
{
    private const string ChineseKeyName = @"HKEY_CURRENT_USER\Software\miHoYo\原神";
    private const string OverseaKeyName = @"HKEY_CURRENT_USER\Software\miHoYo\Genshin Impact";
    private const string SdkChineseValueName = "MIHOYOSDK_ADL_PROD_CN_h3123967166";
    private const string SdkOverseaValueName = "MIHOYOSDK_ADL_PROD_OVERSEA_h1158948810";

    private const string WindowsHDROnValueName = "WINDOWS_HDR_ON_h3132281285";

    public static bool Set(GameAccount? account)
    {
        if (account is not null)
        {
            // 存回注册表的字节需要 '\0' 结尾
            byte[] target = [.. Encoding.UTF8.GetBytes(account.MihoyoSDK), 0];
            (string keyName, string valueName) = GetKeyValueName(account.Type);
            Registry.SetValue(keyName, valueName, target);

            if (Get(account.Type) == account.MihoyoSDK)
            {
                return true;
            }
        }

        return false;
    }

    public static unsafe string? Get(SchemeType scheme)
    {
        (string keyName, string valueName) = GetKeyValueName(scheme);
        object? sdk = Registry.GetValue(keyName, valueName, Array.Empty<byte>());

        if (sdk is not byte[] bytes)
        {
            return null;
        }

        fixed (byte* pByte = bytes)
        {
            // 从注册表获取的字节数组带有 '\0' 结尾，需要舍去
            ReadOnlySpan<byte> span = MemoryMarshal.CreateReadOnlySpanFromNullTerminated(pByte);
            return Encoding.UTF8.GetString(span);
        }
    }

    public static void SetWindowsHDR(bool isOversea)
    {
        string keyName = isOversea ? OverseaKeyName : ChineseKeyName;
        Registry.SetValue(keyName, WindowsHDROnValueName, 1);
    }

    private static (string KeyName, string ValueName) GetKeyValueName(SchemeType scheme)
    {
        return scheme switch
        {
            SchemeType.ChineseOfficial => (ChineseKeyName, SdkChineseValueName),
            SchemeType.Oversea => (OverseaKeyName, SdkOverseaValueName),
            _ => throw ThrowHelper.NotSupported($"Invalid account SchemeType: {scheme}"),
        };
    }
}