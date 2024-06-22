// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.DataTransfer;
using Snap.Hutao.Service.Notification;

namespace Snap.Hutao.Web.Hoyolab.HoyoPlay.Connect;

internal partial class PackageSegment
{
    [JsonPropertyName("url")]
    public string Url { get; set; } = default!;

    [JsonPropertyName("md5")]
    public string MD5 { get; set; } = default!;

    [JsonPropertyName("size")]
    public long Size { get; set; } = default!;

    [JsonPropertyName("decompressed_size")]
    public long DecompressedSize { get; set; } = default!;

    [JsonIgnore]
    public string DisplayName { get => System.IO.Path.GetFileName(Url); }

    [Command("CopyPathCommand")]
    private void CopyPathToClipboard()
    {
        IServiceProvider serviceProvider = Ioc.Default;
        serviceProvider.GetRequiredService<IClipboardProvider>().SetText(Url);
        serviceProvider.GetRequiredService<IInfoBarService>().Success(SH.WebGameResourcePathCopySucceed);
    }
}
