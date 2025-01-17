// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Google.Protobuf;
using Snap.Hutao.Model.Entity;
using System.Collections.Immutable;

namespace Snap.Hutao.Service.Yae.PlayerStore;

internal static class PlayerStoreParser
{
    public static ImmutableArray<InventoryItem> Parse(byte[] bytes, CultivateProject project)
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

        return items.Where(i => i.Material is not null).Select(
            i => new InventoryItem()
            {
                ProjectId = project.InnerId,
                ItemId = i.ItemId,
                Count = i.Material.Count,
            }).ToImmutableArray();
    }
}