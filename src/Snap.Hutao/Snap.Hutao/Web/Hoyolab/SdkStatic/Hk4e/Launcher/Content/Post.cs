// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.SdkStatic.Hk4e.Launcher.Content;

internal sealed class Post
{
    [JsonPropertyName("post_id")]
    public string PostId { get; set; } = default!;

    /// <summary>
    /// POST_TYPE_ACTIVITY|POST_TYPE_ANNOUNCE|POST_TYPE_INFO
    /// </summary>
    [JsonPropertyName("type")]
    public string Type { get; set; } = default!;

    [JsonPropertyName("url")]
    public string Url { get; set; } = default!;

    /// <summary>
    /// MM/dd format
    /// </summary>
    [JsonPropertyName("show_time")]
    public string ShowTime { get; set; } = default!;

    [JsonPropertyName("order")]
    public string Order { get; set; } = default!;

    [JsonPropertyName("title")]
    public string Title { get; set; } = default!;
}