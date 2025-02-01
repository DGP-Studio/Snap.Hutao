// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Google.Protobuf;
using Snap.Hutao.Core.Protobuf;
using Snap.Hutao.Model.InterChange.Inventory;

namespace Snap.Hutao.Service.Yae.PlayerStore;

internal static class PlayerStoreParser
{
    public static UIIF? Parse(ByteString bytes)
    {
        List<Item> items = [];
        using (CodedInputStream stream = bytes.CreateCodedInput())
        {
            try
            {
                while (stream.TryReadTag(out uint tag))
                {
                    switch (WireFormat.GetTagWireType(tag))
                    {
                        case WireFormat.WireType.Varint:
                            {
                                _ = stream.ReadUInt32();
                                continue;
                            }

                        case WireFormat.WireType.LengthDelimited:
                            {
                                using (CodedInputStream inputStream = stream.UnsafeReadLengthDelimitedStream())
                                {
                                    while (inputStream.TryPeekTag(out _))
                                    {
                                        items.Add(Item.Parser.ParseFrom(inputStream));
                                    }
                                }

                                break;
                            }
                    }
                }
            }
            catch (InvalidProtocolBufferException)
            {
                return default;
            }

            return new()
            {
                Info = UIIFInfo.CreateForEmbeddedYae(),
                List = [.. items.Select(UIIFItem.FromInGameItem)],
            };
        }
    }
}