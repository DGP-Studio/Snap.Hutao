// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Text.Json.Annotation;
using System.Runtime.CompilerServices;

namespace Snap.Hutao.Core.Text.Json.Converter;

internal sealed class UnsafeEnumConverter<TEnum> : JsonConverter<TEnum>
    where TEnum : struct, Enum
{
    private readonly TypeCode enumTypeCode = Type.GetTypeCode(typeof(TEnum));

    private readonly JsonEnumHandling readAs;
    private readonly JsonEnumHandling writeAs;

    public UnsafeEnumConverter(JsonEnumHandling readAs, JsonEnumHandling writeAs)
    {
        this.readAs = readAs;
        this.writeAs = writeAs;
    }

    public override TEnum Read(ref Utf8JsonReader reader, Type typeToConverTEnum, JsonSerializerOptions options)
    {
        if (readAs is JsonEnumHandling.Number)
        {
            return GetEnum(ref reader, enumTypeCode);
        }

        if (reader.GetString() is { } str)
        {
            return Enum.Parse<TEnum>(str, ignoreCase: true);
        }

        throw new JsonException();
    }

    public override void Write(Utf8JsonWriter writer, TEnum value, JsonSerializerOptions options)
    {
        switch (writeAs)
        {
            case JsonEnumHandling.Number:
                WriteNumberValue(writer, value, enumTypeCode);
                break;
            case JsonEnumHandling.NumberString:
                writer.WriteStringValue(value.ToString("D"));
                break;
            default:
                writer.WriteStringValue(value.ToString());
                break;
        }
    }

    private static TEnum GetEnum(ref Utf8JsonReader reader, TypeCode typeCode)
    {
        switch (typeCode)
        {
            case TypeCode.Int32:
                if (reader.TryGetInt32(out int int32))
                {
                    return Unsafe.As<int, TEnum>(ref int32);
                }

                break;
            case TypeCode.UInt32:
                if (reader.TryGetUInt32(out uint uint32))
                {
                    return Unsafe.As<uint, TEnum>(ref uint32);
                }

                break;
            case TypeCode.UInt64:
                if (reader.TryGetUInt64(out ulong uint64))
                {
                    return Unsafe.As<ulong, TEnum>(ref uint64);
                }

                break;
            case TypeCode.Int64:
                if (reader.TryGetInt64(out long int64))
                {
                    return Unsafe.As<long, TEnum>(ref int64);
                }

                break;
            case TypeCode.Byte:
                if (reader.TryGetByte(out byte byte8))
                {
                    return Unsafe.As<byte, TEnum>(ref byte8);
                }

                break;
            case TypeCode.Int16:
                if (reader.TryGetInt16(out short int16))
                {
                    return Unsafe.As<short, TEnum>(ref int16);
                }

                break;
            case TypeCode.UInt16:
                if (reader.TryGetUInt16(out ushort uint16))
                {
                    return Unsafe.As<ushort, TEnum>(ref uint16);
                }

                break;
        }

        throw new JsonException();
    }

    private static void WriteNumberValue(Utf8JsonWriter writer, TEnum value, TypeCode typeCode)
    {
        switch (typeCode)
        {
            case TypeCode.Int32:
                writer.WriteNumberValue(Unsafe.As<TEnum, int>(ref value));
                break;
            case TypeCode.UInt32:
                writer.WriteNumberValue(Unsafe.As<TEnum, uint>(ref value));
                break;
            case TypeCode.UInt64:
                writer.WriteNumberValue(Unsafe.As<TEnum, ulong>(ref value));
                break;
            case TypeCode.Int64:
                writer.WriteNumberValue(Unsafe.As<TEnum, long>(ref value));
                break;
            case TypeCode.Int16:
                writer.WriteNumberValue(Unsafe.As<TEnum, short>(ref value));
                break;
            case TypeCode.UInt16:
                writer.WriteNumberValue(Unsafe.As<TEnum, ushort>(ref value));
                break;
            case TypeCode.Byte:
                writer.WriteNumberValue(Unsafe.As<TEnum, byte>(ref value));
                break;
            case TypeCode.SByte:
                writer.WriteNumberValue(Unsafe.As<TEnum, sbyte>(ref value));
                break;
            default:
                throw new JsonException();
        }
    }
}