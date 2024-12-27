// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Primitive;
using System.Collections.Immutable;

namespace Snap.Hutao.Web.Hoyolab.Takumi.GameRecord.Avatar;

internal sealed class CharacterData
{
    public CharacterData(PlayerUid uid)
    {
        Uid = uid.Value;
        Server = uid.Region;
    }

    public CharacterData(PlayerUid uid, ImmutableArray<AvatarId> characterIds)
        : this(uid)
    {
        CharacterIds = characterIds;
    }

    [JsonPropertyName("sort_type")]
    public uint SortType { get; } = 1;

    [JsonPropertyName("character_ids")]
    public ImmutableArray<AvatarId>? CharacterIds { get; }

    [JsonPropertyName("role_id")]
    public string Uid { get; }

    [JsonPropertyName("server")]
    public Region Server { get; }
}