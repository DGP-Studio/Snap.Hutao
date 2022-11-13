// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Win32;
using Snap.Hutao.Core.Convert;
using Snap.Hutao.Extension;
using Snap.Hutao.Web.Hoyolab.DynamicSecret;
using System.Collections.Immutable;
using System.Text.Encodings.Web;
using Windows.ApplicationModel;

namespace Snap.Hutao.Core;

/// <summary>
/// 核心环境参数
/// </summary>
internal static class CoreEnvironment
{
    // 计算过程：https://github.com/UIGF-org/Hoyolab.Salt

    /// <summary>
    /// 动态密钥1的K2盐
    /// </summary>
    public const string DynamicSecretK2Salt = "fdv0fY9My9eA7MR0NpjGP9RjueFvjUSQ";

    /// <summary>
    /// 动态密钥1的LK2盐
    /// </summary>
    public const string DynamicSecretLK2Salt = "jEpJb9rRARU2rXDA9qYbZ3selxkuct9a";

    /// <summary>
    /// 动态密钥2的X4盐
    /// </summary>
    public const string DynamicSecretX4Salt = "xV8v4Qu54lUKrEYFZkJhB8cuOh9Asafs";

    /// <summary>
    /// 动态密钥2的X6盐
    /// </summary>
    public const string DynamicSecretX6Salt = "t0qEgfub6cvueAPgR5m9aQWWVciEer7v";

    /// <summary>
    /// LoginApi的盐
    /// </summary>
    public const string DynamicSecretPRODSalt = "JwYDpKvLj6MrMqqYU6jTKF17KNO2PXoS";

    /// <summary>
    /// 米游社请求UA
    /// </summary>
    public const string HoyolabUA = $"Mozilla/5.0 (Windows NT 10.0; Win64; x64) miHoYoBBS/{HoyolabXrpcVersion}";

    /// <summary>
    /// 米游社 Rpc 版本
    /// </summary>
    public const string HoyolabXrpcVersion = "2.40.1";

    /// <summary>
    /// 动态密钥
    /// </summary>
    public static readonly ImmutableDictionary<SaltType, string> DynamicSecrets = new Dictionary<SaltType, string>()
    {
        [SaltType.K2] = "fdv0fY9My9eA7MR0NpjGP9RjueFvjUSQ",
        [SaltType.LK2] = "jEpJb9rRARU2rXDA9qYbZ3selxkuct9a",
        [SaltType.X4] = "xV8v4Qu54lUKrEYFZkJhB8cuOh9Asafs",
        [SaltType.X6] = "t0qEgfub6cvueAPgR5m9aQWWVciEer7v",
        [SaltType.PROD] = "JwYDpKvLj6MrMqqYU6jTKF17KNO2PXoS",
    }.ToImmutableDictionary();

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
    /// 包家族名称
    /// </summary>
    public static readonly string FamilyName;

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
        FamilyName = Package.Current.Id.FamilyName;
        CommonUA = $"Snap Hutao/{Version}";

        // simply assign a random guid
        HoyolabDeviceId = Guid.NewGuid().ToString();
        HutaoDeviceId = GetUniqueUserID();
    }

    private static string GetUniqueUserID()
    {
        string userName = Environment.UserName;
        object? machineGuid = Registry.GetValue(CryptographyKey, MachineGuidValue, userName);
        return Md5Convert.ToHexString($"{userName}{machineGuid}");
    }
}