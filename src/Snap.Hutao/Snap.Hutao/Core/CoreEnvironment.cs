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
    /// 动态密钥1的盐
    /// </summary>
    public const string DynamicSecret1Salt = "9nQiU3AV0rJSIBWgdynfoGMGKaklfbM7";

    /// <summary>
    /// 动态密钥2的盐
    /// 计算过程：https://gist.github.com/Lightczx/373c5940b36e24b25362728b52dec4fd
    /// </summary>
    public const string DynamicSecret2Salt = "xV8v4Qu54lUKrEYFZkJhB8cuOh9Asafs";

    /// <summary>
    /// 米游社请求UA
    /// </summary>
    public const string HoyolabUA = $"miHoYoBBS/2.34.1";

    /// <summary>
    /// 标准UA
    /// </summary>
    public static readonly string CommonUA;

    /// <summary>
    /// 当前版本
    /// </summary>
    public static readonly Version Version;

    /// <summary>
    /// 设备Id
    /// </summary>
    public static readonly string DeviceId;

    /// <summary>
    /// 米游社设备Id
    /// </summary>
    public static readonly string HoyolabDeviceId;

    private const string CryptographyKey = @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Cryptography";
    private const string MachineGuidValue = "MachineGuid";

    static CoreEnvironment()
    {
        Version = Package.Current.Id.Version.ToVersion();
        CommonUA = $"Snap Hutao/{Version}";
        DeviceId = GetDeviceId();
        HoyolabDeviceId = Guid.NewGuid().ToString();
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