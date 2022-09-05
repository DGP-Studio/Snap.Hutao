// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Extension;
using Windows.ApplicationModel;

namespace Snap.Hutao.Core;

/// <summary>
/// 核心环境参数
/// </summary>
internal static class CoreEnvironment
{
    /// <summary>
    /// 动态密钥的盐
    /// 计算过程：https://gist.github.com/Lightczx/373c5940b36e24b25362728b52dec4fd
    /// </summary>
    public const string DynamicSecretSalt = "xV8v4Qu54lUKrEYFZkJhB8cuOh9Asafs";

    /// <summary>
    /// 米游社请求UA
    /// </summary>
    public const string HoyolabUA = $"miHoYoBBS/{HoyolabXrpcVersion}";

    /// <summary>
    /// 米游社 Rpc 版本
    /// </summary>
    public const string HoyolabXrpcVersion = "2.36.1";

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

    static CoreEnvironment()
    {
        Version = Package.Current.Id.Version.ToVersion();
        CommonUA = $"Snap Hutao/{Version}";

        // simply assign a random guid
        HoyolabDeviceId = Guid.NewGuid().ToString();
    }
}