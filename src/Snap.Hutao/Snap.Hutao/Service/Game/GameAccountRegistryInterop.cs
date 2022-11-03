// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Win32;
using Snap.Hutao.Model.Entity;
using System.Text;

namespace Snap.Hutao.Service.Game;

/// <summary>
/// 定义了对注册表的操作
/// </summary>
internal static class GameAccountRegistryInterop
{
    private const string GenshinKey = @"HKEY_CURRENT_USER\Software\miHoYo\原神";
    private const string SdkKey = "MIHOYOSDK_ADL_PROD_CN_h3123967166";

    /// <summary>
    /// 设置键值
    /// </summary>
    /// <param name="account">账户</param>
    /// <returns>账号是否设置</returns>
    public static bool Set(GameAccount? account)
    {
        if (account != null)
        {
            Registry.SetValue(GenshinKey, SdkKey, Encoding.UTF8.GetBytes(account.MihoyoSDK));
            return true;
        }

        return false;
    }

    /// <summary>
    /// 在注册表中获取账号信息
    /// </summary>
    /// <returns>当前注册表中的信息</returns>
    public static string? Get()
    {
        object? sdk = Registry.GetValue(GenshinKey, SdkKey, Array.Empty<byte>());

        if (sdk is byte[] bytes)
        {
            return Encoding.UTF8.GetString(bytes);
        }

        return null;
    }
}