// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Google.Protobuf;
using Snap.Hutao.Model.InterChange.Inventory;
using System.Collections.Immutable;

namespace Snap.Hutao.Service.Yae.PlayerStore;

internal static class PlayerStoreParser
{
    public static UIIF? Parse(byte[] bytes)
    {
        List<Item> items = [];
        using CodedInputStream stream = new(bytes);
        try
        {
            uint tag;
            while ((tag = stream.ReadTag()) != 0)
            {
                uint wireType = tag & 7;
                switch (wireType)
                {
                    case 0:
                        {
                            // is VarInt
                            _ = stream.ReadUInt32();
                            continue;
                        }

                    case 2:
                        {
                            // is LengthDelimited
                            using CodedInputStream eStream = stream.ReadLengthDelimitedAsStream();
                            while (eStream.PeekTag() != 0)
                            {
                                items.Add(Item.Parser.ParseFrom(eStream));
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
            Info = UIIFInfo.CreateForYaeLib(),
            List = items.Select(UIIFItem.FromPlayerStoreItem).ToImmutableArray(),
        };
    }
}