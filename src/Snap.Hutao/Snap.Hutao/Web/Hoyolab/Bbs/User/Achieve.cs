// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Text.Json.Serialization;

namespace Snap.Hutao.Web.Hoyolab.Bbs.User;

/// <summary>
/// 成就
/// </summary>
public class Achieve
{
    /// <summary>
    /// 获赞数
    /// </summary>
    [JsonPropertyName("like_num")]
    public string LikeNum { get; set; } = default!;

    /// <summary>
    /// 帖子数
    /// </summary>
    [JsonPropertyName("post_num")]
    public string PostNum { get; set; } = default!;

    /// <summary>
    /// 帖子回复数
    /// </summary>
    [JsonPropertyName("replypost_num")]
    public string ReplypostNum { get; set; } = default!;

    /// <summary>
    /// 关注数
    /// </summary>
    [JsonPropertyName("follow_cnt")]
    public string FollowCnt { get; set; } = default!;

    /// <summary>
    /// 粉丝
    /// </summary>
    [JsonPropertyName("followed_cnt")]
    public string FollowedCnt { get; set; } = default!;

    /// <summary>
    /// 话题数量
    /// </summary>
    [JsonPropertyName("topic_cnt")]
    public string TopicCnt { get; set; } = default!;

    /// <summary>
    /// 新增粉丝数
    /// </summary>
    [JsonPropertyName("new_follower_num")]
    public string NewFollowerNum { get; set; } = default!;

    /// <summary>
    /// 精品帖个数
    /// </summary>
    [JsonPropertyName("good_post_num")]
    public string GoodPostNum { get; set; } = default!;

    /// <summary>
    /// 合集个数
    /// </summary>
    [JsonPropertyName("follow_collection_cnt")]
    public string FollowCollectionCnt { get; set; } = default!;
}
