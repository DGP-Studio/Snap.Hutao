﻿// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.IO.DataTransfer;
using Snap.Hutao.Service.Notification;

namespace Snap.Hutao.Web.Hoyolab.SdkStatic.Hk4e.Launcher;

/// <summary>
/// 下载的文件
/// </summary>
[HighQuality]
internal partial class PathMd5
{
    /// <summary>
    /// 下载地址
    /// </summary>
    [JsonPropertyName("path")]
    public string Path { get; set; } = default!;

    /// <summary>
    /// MD5校验
    /// </summary>
    [JsonPropertyName("md5")]
    public string Md5 { get; set; } = default!;

    /// <summary>
    /// 显示名称
    /// </summary>
    public string DisplayName { get => System.IO.Path.GetFileName(Path); }

    [Command("CopyPathCommand")]
    private void CopyPathToClipboard()
    {
        IServiceProvider serviceProvider = Ioc.Default;
        serviceProvider.GetRequiredService<IClipboardInterop>().SetText(Path);
        serviceProvider.GetRequiredService<IInfoBarService>().Success(SH.WebGameResourcePathCopySucceed);
    }
}
