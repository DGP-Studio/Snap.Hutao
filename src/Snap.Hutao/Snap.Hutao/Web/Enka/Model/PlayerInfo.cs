// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Primitive;

namespace Snap.Hutao.Web.Enka.Model;

internal sealed class PlayerInfo
{
    [JsonPropertyName("nickname")]
    public string Nickname { get; set; } = default!;

    [JsonPropertyName("level")]
    public uint Level { get; set; }

    [JsonPropertyName("signature")]
    public string Signature { get; set; } = default!;

    [JsonPropertyName("worldLevel")]
    public uint WorldLevel { get; set; }

    [JsonPropertyName("nameCardId")]
    public MaterialId NameCardId { get; set; }

    [JsonPropertyName("finishAchievementNum")]
    public uint FinishAchievementNum { get; set; }

    [JsonPropertyName("towerFloorIndex")]
    public uint TowerFloorIndex { get; set; }

    [JsonPropertyName("towerLevelIndex")]
    public uint TowerLevelIndex { get; set; }

    [JsonPropertyName("showAvatarInfoList")]
    public List<ShowAvatarInfo> ShowAvatarInfoList { get; set; } = default!;

    [JsonPropertyName("showNameCardIdList")]
    public List<MaterialId> ShowNameCardIdList { get; set; } = default!;

    [JsonPropertyName("profilePicture")]
    public ProfilePicture ProfilePicture { get; set; } = default!;
}