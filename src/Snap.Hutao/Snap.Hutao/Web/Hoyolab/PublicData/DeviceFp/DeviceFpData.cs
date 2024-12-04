// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.PublicData.DeviceFp;

internal sealed class DeviceFpData
{
    [JsonPropertyName("device_id")]
    public string DeviceId { get; set; } = default!;

    [JsonPropertyName("bbs_device_id")]
    public string? BbsDeviceId { get; set; }

    [JsonPropertyName("seed_id")]
    public string SeedId { get; set; } = default!;

    [JsonPropertyName("seed_time")]
    public string SeedTime { get; set; } = default!;

    [JsonPropertyName("platform")]
    public string Platform { get; set; } = default!;

    [JsonPropertyName("device_fp")]
    public string DeviceFp { get; set; } = default!;

    [JsonPropertyName("app_name")]
    public string AppName { get; set; } = default!;

    [JsonPropertyName("ext_fields")]
    public string ExtFields { get; set; } = default!;
}