// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Win32;
using Snap.Hutao.Core.ExceptionService;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Model.Entity.Primitive;
using System.Runtime.InteropServices;
using System.Text;

namespace Snap.Hutao.Service.Game.Account;

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
            (string keyName, string valueName) = GetKeyValueName(account.Type);
            byte[] target = [.. Encoding.UTF8.GetBytes(account.MihoyoSDK), 0];
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
            _ => throw HutaoException.NotSupported($"Unsupported account SchemeType: {scheme}"),
        };
    }
}