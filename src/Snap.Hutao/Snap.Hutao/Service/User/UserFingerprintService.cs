// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.ViewModel.User;
using Snap.Hutao.Web.Hoyolab;
using Snap.Hutao.Web.Hoyolab.PublicData.DeviceFp;
using Snap.Hutao.Web.Response;

namespace Snap.Hutao.Service.User;

[Service(ServiceLifetime.Singleton, typeof(IUserFingerprintService))]
internal sealed partial class UserFingerprintService : IUserFingerprintService
{
    private readonly IServiceScopeFactory serviceScopeFactory;

    [GeneratedConstructor]
    public partial UserFingerprintService(IServiceProvider serviceProvider);

    public async ValueTask TryInitializeAsync(ViewModel.User.User user, CancellationToken token = default)
    {
        if (user.IsOversea)
        {
            // Disable HoYoLAB fp approach
            return;
        }

        if (user.Entity.FingerprintLastUpdateTime >= DateTimeOffset.UtcNow - TimeSpan.FromDays(7))
        {
            if (!string.IsNullOrEmpty(user.Fingerprint))
            {
                return;
            }
        }

        string device = Core.Random.GetUpperAndNumberString(12);
        string product = Core.Random.GetUpperAndNumberString(6);

        // To anyone who finds this code, please don't fully copy this.
        Dictionary<string, object> extendProperties = new()
        {
            { "proxyStatus", 0 },
            { "isRoot", 0 },
            { "romCapacity", "512" },
            { "deviceName", device },
            { "productName", product },
            { "romRemain", "512" },
            { "hostname", "dg02-pool03-kvm87" },
            { "screenSize", "1440x2905" },
            { "isTablet", 0 },
            { "aaid", string.Empty },
            { "model", device },
            { "brand", "XiaoMi" },
            { "hardware", "qcom" },
            { "deviceType", "OP5913L1" },
            { "devId", "REL" },
            { "serialNumber", "unknown" },
            { "sdCapacity", 512215 },
            { "buildTime", "1693626947000" },
            { "buildUser", "android-build" },
            { "simState", 5 },
            { "ramRemain", "239814" },
            { "appUpdateTimeDiff", 1702604034482 },
            { "deviceInfo", $@"XiaoMi\/{product}\/OP5913L1:13\/SKQ1.221119.001\/T.118e6c7-5aa23-73911:user\/release-keys" },
            { "vaid", string.Empty },
            { "buildType", "user" },
            { "sdkVersion", "34" },
            { "ui_mode", "UI_MODE_TYPE_NORMAL" },
            { "isMockLocation", 0 },
            { "cpuType", "arm64-v8a" },
            { "isAirMode", 0 },
            { "ringMode", 2 },
            { "chargeStatus", 1 },
            { "manufacturer", "XiaoMi" },
            { "emulatorStatus", 0 },
            { "appMemory", "512" },
            { "osVersion", "14" },
            { "vendor", "unknown" },
            { "accelerometer", "1.4883357x7.1712894x6.2847486" },
            { "sdRemain",  239600 },
            { "buildTags", "release-keys" },
            { "packageName", "com.mihoyo.hyperion" },
            { "networkType", "WiFi" },
            { "oaid", string.Empty },
            { "debugStatus", 1 },
            { "ramCapacity", "469679" },
            { "magnetometer", "20.081251x-27.487501x2.1937501" },
            { "display", $"{product}_13.1.0.181(CN01)" },
            { "appInstallTimeDiff", 1688455751496 },
            { "packageVersion", "2.20.1" },
            { "gyroscope", "0.030226856x0.014647375x0.010652636" },
            { "batteryStatus", 100 },
            { "hasKeyboard", 0 },
            { "board", "taro" },
        };

        DeviceFpData data = new()
        {
            DeviceId = Core.Random.GetLowerHexString(16),
            SeedId = $"{Guid.NewGuid()}",
            Platform = "2",
            SeedTime = $"{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}",
            ExtFields = JsonSerializer.Serialize(extendProperties),
            AppName = "bbs_cn",
            BbsDeviceId = HoyolabOptions.DeviceId36,
            DeviceFp = string.IsNullOrEmpty(user.Fingerprint) ? Core.Random.GetLowerHexString(13) : user.Fingerprint,
        };

        using (IServiceScope scope = serviceScopeFactory.CreateScope())
        {
            DeviceFpClient deviceFpClient = scope.ServiceProvider.GetRequiredService<DeviceFpClient>();
            Response<DeviceFpWrapper> response = await deviceFpClient.GetFingerprintAsync(data, token).ConfigureAwait(false);

            ResponseValidator.TryValidate(response, scope.ServiceProvider, out DeviceFpWrapper? wrapper);
            user.TryUpdateFingerprint(wrapper?.DeviceFp ?? string.Empty);

            user.NeedDbUpdateAfterResume = true;
        }
    }
}