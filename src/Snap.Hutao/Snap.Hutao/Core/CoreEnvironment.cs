// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Win32;
using Snap.Hutao.Core.Json;
using Snap.Hutao.Core.Setting;
using Snap.Hutao.Web.Hoyolab.DynamicSecret;
using System.Collections.Immutable;
using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json.Serialization.Metadata;
using Windows.ApplicationModel;

namespace Snap.Hutao.Core;

/// <summary>
/// 核心环境参数
/// </summary>
[HighQuality]
internal static class CoreEnvironment
{
    /// <summary>
    /// 米游社请求UA
    /// </summary>
    public const string HoyolabUA = $"Mozilla/5.0 (Windows NT 10.0; Win64; x64) miHoYoBBS/{HoyolabXrpcVersion}";

    /// <summary>
    /// 米游社移动端请求UA
    /// </summary>
    public const string HoyolabMobileUA = $"Mozilla/5.0 (Linux; Android 12) Mobile miHoYoBBS/{HoyolabXrpcVersion}";

    /// <summary>
    /// Hoyolab iPhone 移动端请求UA
    /// </summary>
    public const string HoyolabOsMobileUA = "Mozilla/5.0 (iPhone; CPU iPhone OS 16_2 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) miHoYoBBSOversea/2.28.0";

    /// <summary>
    /// 米游社 Rpc 版本
    /// </summary>
    public const string HoyolabXrpcVersion = "2.44.1";

    /// <summary>
    /// 盐
    /// </summary>
    // https://github.com/UIGF-org/Hoyolab.Salt
    public static readonly ImmutableDictionary<SaltType, string> DynamicSecretSalts = new Dictionary<SaltType, string>()
    {
        [SaltType.K2] = "dZAwGk4e9aC0MXXItkwnHamjA1x30IYw",
        [SaltType.LK2] = "IEIZiKYaput2OCKQprNuGsog1NZc1FkS",
        [SaltType.X4] = "xV8v4Qu54lUKrEYFZkJhB8cuOh9Asafs",
        [SaltType.X6] = "t0qEgfub6cvueAPgR5m9aQWWVciEer7v",
        [SaltType.PROD] = "JwYDpKvLj6MrMqqYU6jTKF17KNO2PXoS",
        [SaltType.OS] = "6cqshh5dhw73bzxn20oexa9k516chk7s",
    }.ToImmutableDictionary();

    /// <summary>
    /// 默认的Json序列化选项
    /// </summary>
    public static readonly JsonSerializerOptions JsonOptions = new()
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        PropertyNameCaseInsensitive = true,
        WriteIndented = true,
        TypeInfoResolver = new DefaultJsonTypeInfoResolver()
        {
            Modifiers =
            {
                JsonTypeInfoResolvers.ResolveEnumType,
            },
        },
    };

    /// <summary>
    /// 当前版本
    /// </summary>
    public static readonly Version Version;

    /// <summary>
    /// 标准UA
    /// </summary>
    public static readonly string CommonUA;

    /// <summary>
    /// 数据文件夹
    /// </summary>
    public static readonly string DataFolder;

    /// <summary>
    /// 包家族名称
    /// </summary>
    public static readonly string FamilyName;

    /// <summary>
    /// 米游社设备Id
    /// </summary>
    public static readonly string HoyolabDeviceId;

    /// <summary>
    /// 胡桃设备Id
    /// </summary>
    public static readonly string HutaoDeviceId;

    /// <summary>
    /// 安装位置
    /// </summary>
    public static readonly string InstalledLocation;

    private const string CryptographyKey = @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Cryptography\";
    private const string MachineGuidValue = "MachineGuid";

    static CoreEnvironment()
    {
        DataFolder = GetDatafolderPath();
        Version = Package.Current.Id.Version.ToVersion();
        FamilyName = Package.Current.Id.FamilyName;
        InstalledLocation = Package.Current.InstalledLocation.Path;
        CommonUA = $"Snap Hutao/{Version}";

        // simply assign a random guid
        HoyolabDeviceId = Guid.NewGuid().ToString();
        HutaoDeviceId = GetUniqueUserID();
    }

    private static string GetUniqueUserID()
    {
        string userName = Environment.UserName;
        object? machineGuid = Registry.GetValue(CryptographyKey, MachineGuidValue, userName);
        return Convert.ToMd5HexString($"{userName}{machineGuid}");
    }

    private static string GetDatafolderPath()
    {
        string preferredPath = LocalSetting.Get(SettingKeys.DataFolderPath, string.Empty);

        if (string.IsNullOrEmpty(preferredPath))
        {
            string myDocument = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
#if RELEASE
            // 将测试版与正式版的文件目录分离
            string folderName = Package.Current.PublisherDisplayName == "DGP Studio CI" ? "HutaoAlpha" : "Hutao";
#else
            // 使得迁移能正常生成
            string folderName = "Hutao";
#endif
            string path = Path.GetFullPath(Path.Combine(myDocument, folderName));
            Directory.CreateDirectory(path);
            return path;
        }
        else
        {
            return preferredPath;
        }
    }
}