// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Win32;
using Snap.Hutao.Extension;
using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;
using Windows.ApplicationModel;

namespace Snap.Hutao.Core;

/// <summary>
/// 核心环境参数
/// </summary>
internal static class CoreEnvironment
{
    // 计算过程：https://gist.github.com/Lightczx/373c5940b36e24b25362728b52dec4fd

    /// <summary>
    /// 动态密钥1的盐
    /// </summary>
    public const string DynamicSecret1Salt = "yUZ3s0Sna1IrSNfk29Vo6vRapdOyqyhB";

    /// <summary>
    /// 动态密钥2的盐
    /// </summary>
    public const string DynamicSecret2Salt = "xV8v4Qu54lUKrEYFZkJhB8cuOh9Asafs";

    /// <summary>
    /// 米游社请求UA
    /// </summary>
    public const string HoyolabUA = $"Mozilla/5.0 (Windows NT 10.0; Win64; x64) miHoYoBBS/{HoyolabXrpcVersion}";

    /// <summary>
    /// 米游社 Rpc 版本
    /// </summary>
    public const string HoyolabXrpcVersion = "2.38.1";

    /// <summary>
    /// 标准UA
    /// </summary>
    public static readonly string CommonUA;

    /// <summary>
    /// 当前版本
    /// </summary>
    public static readonly Version Version;

    /// <summary>
    /// 米游社设备Id
    /// </summary>
    public static readonly string HoyolabDeviceId;

    /// <summary>
    /// 胡桃设备Id
    /// </summary>
    public static readonly string HutaoDeviceId;

    /// <summary>
    /// 默认的Json序列化选项
    /// </summary>
    public static readonly JsonSerializerOptions JsonOptions = new()
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        PropertyNameCaseInsensitive = true,
        WriteIndented = true,
    };

    private const string CryptographyKey = @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Cryptography\";
    private const string MachineGuidValue = "MachineGuid";

    static CoreEnvironment()
    {
        Version = Package.Current.Id.Version.ToVersion();
        CommonUA = $"Snap Hutao/{Version}";

        // simply assign a random guid
        HoyolabDeviceId = Guid.NewGuid().ToString();
        HutaoDeviceId = GetUniqueUserID();
    }

    private static string GetUniqueUserID()
    {
        string userName = Environment.UserName;
        object? machineGuid = Registry.GetValue(CryptographyKey, MachineGuidValue, userName);
        byte[] bytes = Encoding.UTF8.GetBytes($"{userName}{machineGuid}");
        byte[] hash = MD5.Create().ComputeHash(bytes);
        return System.Convert.ToHexString(hash);
    }
}