// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Intrinsic;
using Snap.Hutao.Model.Primitive;

namespace Snap.Hutao.Web.Hoyolab.Takumi.GameRecord.Avatar;

// Index
internal class Avatar
{
    [JsonPropertyName("id")]
    public AvatarId Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = default!;

    [JsonPropertyName("element")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public ElementName Element { get; set; }

    [JsonPropertyName("fetter")]
    public FetterLevel Fetter { get; set; }

    [JsonPropertyName("level")]
    public Level Level { get; set; }

    [JsonPropertyName("rarity")]
    public QualityType Rarity { get; set; }

    [JsonPropertyName("actived_constellation_num")]
    public int ActivedConstellationNum { get; set; }

    // Not in response data
    [JsonPropertyName("promote_level")]
    public PromoteLevel PromoteLevel { get; set; }

    // Ignored field: string image
    // Ignored field: string card_image
    // Ignored field: bool is_chosen
}