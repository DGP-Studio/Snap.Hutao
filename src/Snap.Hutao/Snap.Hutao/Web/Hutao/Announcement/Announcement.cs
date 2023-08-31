// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.DependencyInjection.Annotation.HttpClient;
using Snap.Hutao.Web.Response;
using System.Net.Http;

namespace Snap.Hutao.Web.Hutao.Announcement;

/// <summary>
/// 胡桃公告
/// </summary>
internal sealed class Announcement : UploadAnnouncement
{
    /// <summary>
    /// Id
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// 语言
    /// </summary>
    public string Locale { get; set; } = default!;

    /// <summary>
    /// 最后更新日期
    /// </summary>
    public long LastUpdateTime { get; set; }
}