// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core;
using System.Globalization;

namespace Snap.Hutao.Service.AppCenter.Model;

[SuppressMessage("", "SA1600")]
public class Device
{
    [JsonPropertyName("sdkName")]
    public string SdkName { get; set; } = "appcenter.winui";

    [JsonPropertyName("sdkVersion")]
    public string SdkVersion { get; set; } = "4.5.0";

    [JsonPropertyName("osName")]
    public string OsName { get; set; } = "WINDOWS";

    [JsonPropertyName("osVersion")]
    public string OsVersion { get; set; } = DeviceHelper.GetSystemVersion();

    [JsonPropertyName("osBuild")]
    public string OsBuild { get; set; } = $"{DeviceHelper.GetSystemVersion()}.{DeviceHelper.GetSystemBuild()}";

    [JsonPropertyName("model")]
    public string? Model { get; set; } = DeviceHelper.GetModel();

    [JsonPropertyName("oemName")]
    public string? OemName { get; set; } = DeviceHelper.GetOem();

    [JsonPropertyName("screenSize")]
    public string ScreenSize { get; set; } = DeviceHelper.GetScreenSize();

    [JsonPropertyName("carrierCountry")]
    public string Country { get; set; } = DeviceHelper.GetCountry() ?? "CN";

    [JsonPropertyName("locale")]
    public string Locale { get; set; } = CultureInfo.CurrentCulture.Name;

    [JsonPropertyName("timeZoneOffset")]
    public int TimeZoneOffset { get; set; } = (int)TimeZoneInfo.Local.BaseUtcOffset.TotalMinutes;

    [JsonPropertyName("appVersion")]
    public string AppVersion { get; set; } = CoreEnvironment.Version.ToString();

    [JsonPropertyName("appBuild")]
    public string AppBuild { get; set; } = CoreEnvironment.Version.ToString();

    [JsonPropertyName("appNamespace")]
    public string AppNamespace { get; set; } = typeof(App).Namespace ?? string.Empty;
}