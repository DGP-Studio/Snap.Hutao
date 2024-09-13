// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.IO;
using Snap.Hutao.Service;

namespace Snap.Hutao.ViewModel.Setting;

[ConstructorGenerated]
[Injection(InjectAs.Scoped)]
internal sealed partial class SettingDownloadViewModel : Abstraction.ViewModel
{
    private readonly StreamCopySpeedLimiter globalDownloadSpeedLimiter;
    private readonly AppOptions appOptions;

    public AppOptions AppOptions { get => appOptions; }

    public int BytesPerSecondLimit
    {
        get => AppOptions.DownloadBytesPerSecondLimit;
        set
        {
            AppOptions.DownloadBytesPerSecondLimit = value;
            globalDownloadSpeedLimiter.SetRateLimit(value);
        }
    }
}
