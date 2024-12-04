// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.Bbs.User;

internal sealed class Achieve
{
    [JsonPropertyName("like_num")]
    public string LikeNum { get; set; } = default!;

    [JsonPropertyName("post_num")]
    public string PostNum { get; set; } = default!;

    [JsonPropertyName("replypost_num")]
    public string ReplypostNum { get; set; } = default!;

    [JsonPropertyName("follow_cnt")]
    public string FollowCnt { get; set; } = default!;

    [JsonPropertyName("followed_cnt")]
    public string FollowedCnt { get; set; } = default!;

    [JsonPropertyName("topic_cnt")]
    public string TopicCnt { get; set; } = default!;

    [JsonPropertyName("new_follower_num")]
    public string NewFollowerNum { get; set; } = default!;

    [JsonPropertyName("good_post_num")]
    public string GoodPostNum { get; set; } = default!;

    [JsonPropertyName("follow_collection_cnt")]
    public string FollowCollectionCnt { get; set; } = default!;
}
