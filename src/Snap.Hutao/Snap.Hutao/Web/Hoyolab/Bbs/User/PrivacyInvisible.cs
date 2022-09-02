// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.Bbs.User;

/// <summary>
/// 隐私设置为不可见的信息
/// </summary>
public class PrivacyInvisible
{
    /// <summary>
    /// 在个人中心展示我的收藏
    /// </summary>
    [JsonPropertyName("post")]
    public bool Post { get; set; }

    /// <summary>
    /// 记录帖子、动态的浏览历史
    /// </summary>
    [JsonPropertyName("collect")]
    public bool Collect { get; set; }

    /// <summary>
    /// 上传图片时添加水印
    /// </summary>
    [JsonPropertyName("watermark")]
    public bool Watermark { get; set; }

    /// <summary>
    /// 在个人中心展示我的评论
    /// </summary>
    [JsonPropertyName("reply")]
    public bool Reply { get; set; }

    /// <summary>
    /// 在个人中心展示我发布的帖子、动态
    /// </summary>
    [JsonPropertyName("post_and_instant")]
    public bool PostAndInstant { get; set; }
}
