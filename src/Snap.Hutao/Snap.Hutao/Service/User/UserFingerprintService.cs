// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Web.Hoyolab;
using Snap.Hutao.Web.Hoyolab.PublicData.DeviceFp;
using Snap.Hutao.Web.Response;
using System.Text;

namespace Snap.Hutao.Service.User;

[ConstructorGenerated]
[Injection(InjectAs.Singleton, typeof(IUserFingerprintService))]
internal sealed partial class UserFingerprintService : IUserFingerprintService
{
    private readonly DeviceFpClient deviceFpClient;

    public async ValueTask TryInitializeAsync(ViewModel.User.User user, CancellationToken token = default)
    {
        if (user.IsOversea)
        {
            // disable HoYoLAB fp approach
            return;
        }

        if (!string.IsNullOrEmpty(user.Fingerprint))
        {
            return;
        }

        string model = GetRandomStringOfLength(6);
        Dictionary<string, string> extendProperties = new()
        {
            { "cpuType", "arm64-v8a" },
            { "romCapacity", "512" },
            { "productName", model },
            { "romRemain", "256" },
            { "manufacturer", "XiaoMi" },
            { "appMemory", "512" },
            { "hostname", "dg02-pool03-kvm87" },
            { "screenSize", "1080x1920" },
            { "osVersion", "13" },
            { "aaid", string.Empty },
            { "vendor", "中国移动" },
            { "accelerometer", "1.4883357x7.1712894x6.2847486" },
            { "buildTags", "release-keys" },
            { "model", model },
            { "brand", "XiaoMi" },
            { "oaid", string.Empty },
            { "hardware", "qcom" },
            { "deviceType", "OP5913L1" },
            { "devId", "REL" },
            { "serialNumber", "unknown" },
            { "buildTime", "1687848011000" },
            { "buildUser", "root" },
            { "ramCapacity", "469679" },
            { "magnetometer", "20.081251x-27.487501x2.1937501" },
            { "display", $"{model}_13.1.0.181(CN01)" },
            { "ramRemain", "215344" },
            { "deviceInfo", $@"XiaoMi/{model}/OP5913L1:13/SKQ1.221119.001/T.118e6c7-5aa23-73911:user/release-keys" },
            { "gyroscope", "0.030226856x0.014647375x0.010652636" },
            { "vaid", string.Empty },
            { "buildType", "user" },
            { "sdkVersion", "33" },
            { "board", "taro" },
        };

        DeviceFpData data = new()
        {
            DeviceId = GetRandomHexStringOfLength(16),
            SeedId = $"{Guid.NewGuid()}",
            Platform = "2",
            SeedTime = $"{DateTimeOffset.Now.ToUnixTimeMilliseconds()}",
            ExtFields = JsonSerializer.Serialize(extendProperties),
            AppName = "bbs_cn",
            BbsDeviceId = HoyolabOptions.DeviceId,
            DeviceFp = string.IsNullOrEmpty(user.Fingerprint) ? GetRandomHexStringOfLength(13) : user.Fingerprint,
        };

        Response<DeviceFpWrapper> response = await deviceFpClient.GetFingerprintAsync(data, token).ConfigureAwait(false);
        user.Fingerprint = response.IsOk() ? response.Data.DeviceFp : string.Empty;
        user.NeedDbUpdateAfterResume = true;
    }

    private static string GetRandomHexStringOfLength(int length)
    {
        const string RandomRange = "0123456789abcdef";

        StringBuilder sb = new(length);

        for (int i = 0; i < length; i++)
        {
            int pos = Random.Shared.Next(0, RandomRange.Length);
            sb.Append(RandomRange[pos]);
        }

        return sb.ToString();
    }

    private static string GetRandomStringOfLength(int length)
    {
        const string RandomRange = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        StringBuilder sb = new(length);

        for (int i = 0; i < length; i++)
        {
            int pos = Random.Shared.Next(0, RandomRange.Length);
            sb.Append(RandomRange[pos]);
        }

        return sb.ToString();
    }
}