// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Intrinsic;

namespace Snap.Hutao.Model.InterChange.Achievement;

// ReSharper disable once InconsistentNaming
internal sealed class UIAFItem
{
    [JsonPropertyName("id")]
    public uint Id { get; init; }

    [JsonPropertyName("timestamp")]
    public long Timestamp { get; init; }

    [JsonPropertyName("current")]
    public uint Current { get; init; }

    [JsonPropertyName("status")]
    public AchievementStatus Status { get; init; }

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