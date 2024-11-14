// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Intrinsic;

namespace Snap.Hutao.Model.InterChange.Achievement;

internal sealed class UIAFItem
{
    [JsonPropertyName("id")]
    public uint Id { get; set; }

    [JsonPropertyName("timestamp")]
    public long Timestamp { get; set; }

    [JsonPropertyName("current")]
    public uint Current { get; set; }

    [JsonPropertyName("status")]
    public AchievementStatus Status { get; set; }

    public static UIAFItem From(Entity.Achievement source)
    {
        return new()
        {
            Id = source.Id,
            Current = source.Current,
            Status = source.Status,
            Timestamp = source.Time.ToUnixTimeSeconds(),
        };
    }
}