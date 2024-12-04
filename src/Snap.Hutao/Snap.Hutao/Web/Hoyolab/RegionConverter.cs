// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab;

internal sealed class RegionConverter : JsonConverter<Region>
{
    public override Region Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.GetString() is { } regionValue)
        {
            return Region.FromRegionString(regionValue);
        }

        throw new JsonException();
    }

    public override void Write(Utf8JsonWriter writer, Region value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.Value);
    }
}