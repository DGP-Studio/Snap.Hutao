// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Extensions.Options;
using Snap.Hutao.Web.Hoyolab.DynamicSecret;
using System.Collections.Immutable;

namespace Snap.Hutao.Web.Hoyolab;

/// <summary>
/// 米游社选项
/// </summary>
[Injection(InjectAs.Singleton)]
internal sealed class HoyolabOptions : IOptions<HoyolabOptions>
{
    /// <summary>
    /// 米游社请求UA
    /// </summary>
    public const string UserAgent = $"Mozilla/5.0 (Windows NT 10.0; Win64; x64) miHoYoBBS/{XrpcVersion}";

    /// <summary>
    /// Hoyolab 请求UA
    /// </summary>
    public const string UserAgentOversea = $"Mozilla/5.0 (Windows NT 10.0; Win64; x64) miHoYoBBSOversea/{XrpcVersionOversea}";

    /// <summary>
    /// 米游社移动端请求UA
    /// </summary>
    public const string MobileUserAgent = $"Mozilla/5.0 (Linux; Android 12) Mobile miHoYoBBS/{XrpcVersion}";

    /// <summary>
    /// Hoyolab 移动端请求UA
    /// </summary>
    public const string MobileUserAgentOversea = $"Mozilla/5.0 (Linux; Android 12) Mobile miHoYoBBSOversea/{XrpcVersionOversea}";

    /// <summary>
    /// 米游社 Rpc 版本
    /// </summary>
    public const string XrpcVersion = "2.52.1";

    /// <summary>
    /// Hoyolab Rpc 版本
    /// </summary>
    public const string XrpcVersionOversea = "2.31.0";

    private static readonly ImmutableDictionary<SaltType, string> SaltsInner = new Dictionary<SaltType, string>()
    {
        // Chinese
        [SaltType.K2] = "HiwYTTu2ovGcU51ehSXfe22SpNmQumlT",
        [SaltType.LK2] = "QCRgj6bHHQvS0Rz03loexYSXpuiO3DZ6",
        [SaltType.X4] = "xV8v4Qu54lUKrEYFZkJhB8cuOh9Asafs",
        [SaltType.X6] = "t0qEgfub6cvueAPgR5m9aQWWVciEer7v",
        [SaltType.PROD] = "JwYDpKvLj6MrMqqYU6jTKF17KNO2PXoS",

        // Oversea
        [SaltType.OSK2] = "599uqkwc0dlqu3h6epzjzfhgyyrd44ae",
        [SaltType.OSLK2] = "rk4xg2hakoi26nljpr099fv9fck1ah10",
        [SaltType.OSX4] = "h4c1d6ywfq5bsbnbhm1bzq7bxzzv6srt",
        [SaltType.OSX6] = "okr4obncj8bw5a65hbnn5oo6ixjc3l9w",
    }.ToImmutableDictionary();

    private static string? deviceId;

    /// <summary>
    /// 米游社设备Id
    /// </summary>
    public static string DeviceId { get => deviceId ??= Guid.NewGuid().ToString(); }

    /// <summary>
    /// 盐
    /// </summary>
    public static ImmutableDictionary<SaltType, string> Salts { get => SaltsInner; }

    /// <inheritdoc/>
    public HoyolabOptions Value { get => this; }
}