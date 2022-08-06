// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Win32;
using Snap.Hutao.Core.Convert;
using Snap.Hutao.Extension;
using Windows.ApplicationModel;

namespace Snap.Hutao.Core;

/// <summary>
/// 核心环境参数
/// </summary>
internal static class CoreEnvironment
{
    /// <summary>
    /// 当前版本
    /// </summary>
    public static readonly Version Version;

    /// <summary>
    /// 设备Id
    /// </summary>
    public static readonly string DeviceId;

    private const string CryptographyKey = @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Cryptography";
    private const string MachineGuidValue = "MachineGuid";

    static CoreEnvironment()
    {
        Version = Package.Current.Id.Version.ToVersion();
        DeviceId = GetDeviceId();
    }

    /// <summary>
    /// 获取设备的UUID
    /// </summary>
    /// <returns>设备的UUID</returns>
    private static string GetDeviceId()
    {
        string userName = Environment.UserName;
        object? machineGuid = Registry.GetValue(CryptographyKey, MachineGuidValue, userName);
        return Md5Convert.ToHexString($"{userName}{machineGuid}");
    }
}