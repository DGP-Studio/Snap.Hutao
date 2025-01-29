// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Model.InterChange.Inventory;

// ReSharper disable once InconsistentNaming
internal sealed class UIIFItemDetail
{
    [JsonPropertyName("count")]
    public uint Count { get; init; }

    public static UIIFItemDetail? Create(uint? count)
    {
        if (count is null)
        {
            return default;
        }

        return new()
        {
            Count = count.Value,
        };
    }

    public static UIIFItemDetail? Create(long? count)
    {
        if (count is null)
        {
            return default;
        }

        return new()
        {
            Count = (uint)Math.Clamp(count.Value, uint.MinValue, uint.MaxValue),
        };
    }
}