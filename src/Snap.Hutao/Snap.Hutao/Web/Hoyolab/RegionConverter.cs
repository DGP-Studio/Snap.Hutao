// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab;

internal sealed class RegionConverter : JsonConverter<Region>
{
    public override Region Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.GetString() is { } regionValue)
        {
            return Region.FromRegion(regionValue);
        }

        return default;
    }

    public override void Write(Utf8JsonWriter writer, Region value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }
}
