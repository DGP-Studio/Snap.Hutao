// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Win32;
using Snap.Hutao.Core.ExceptionService;
using Snap.Hutao.Core.Text.Json;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Model.Entity.Primitive;
using Snap.Hutao.Web.Response;
using System.Collections.Immutable;
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

    public static unsafe bool Set(GameAccount? account)
    {
        if (account is null)
        {
            return false;
        }

        (string keyName, string valueName) = GetKeyValueName(account.Type);
        byte[] target = [.. Encoding.UTF8.GetBytes(account.MihoyoSDK), 0];
        Registry.SetValue(keyName, valueName, target);

        if (Registry.GetValue(keyName, valueName, Array.Empty<byte>()) is not byte[] bytes)
        {
            return false;
        }

        fixed (byte* pByte = bytes)
        {
            return string.Equals(account.MihoyoSDK, Encoding.UTF8.GetString(MemoryMarshal.CreateReadOnlySpanFromNullTerminated(pByte)), StringComparison.Ordinal);
        }
    }

    public static unsafe bool TryGet(SchemeType scheme, [NotNullWhen(true)] out string? raw, [NotNullWhen(true)] out string? address, out DataWrapper<ImmutableArray<AccountInformation>>? userInfo)
    {
        address = default;
        userInfo = default;

        (string keyName, string valueName) = GetKeyValueName(scheme);
        object? sdk = Registry.GetValue(keyName, valueName, Array.Empty<byte>());

        if (sdk is not byte[] bytes)
        {
            raw = default;
            return false;
        }

        fixed (byte* pByte = bytes)
        {
            raw = Encoding.UTF8.GetString(MemoryMarshal.CreateReadOnlySpanFromNullTerminated(pByte));
        }

        try
        {
            string decoded = RegistryAccountDataListDecoder.Decode(raw, out address);
            userInfo = JsonSerializer.Deserialize<DataWrapper<ImmutableArray<AccountInformation>>>(decoded, JsonOptions.Default);
            return true;
        }
        catch (Exception ex)
        {
            _ = ex;
            return false;
        }
    }

    public static void SetWindowsHDR(bool isOversea)
    {
        Registry.SetValue(isOversea ? OverseaKeyName : ChineseKeyName, WindowsHDROnValueName, 1);
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