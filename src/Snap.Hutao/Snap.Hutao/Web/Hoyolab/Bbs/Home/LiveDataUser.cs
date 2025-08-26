// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Web.Hoyolab.Bbs.User;

namespace Snap.Hutao.Web.Hoyolab.Bbs.Home;

internal sealed class LiveDataUser : UserCommon
{
    [JsonPropertyName("is_following")]
    public required bool IsFollowing { get; init; }

    [JsonPropertyName("is_followed")]
    public required bool IsFollowed { get; init; }

    [JsonPropertyName("is_creator")]
    public required bool IsCreator { get; init; }
}