// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Google.Protobuf;
using Snap.Hutao.Model.InterChange.Achievement;
using Snap.Hutao.Model.Intrinsic;
using System.Collections.Immutable;

namespace Snap.Hutao.Service.Yae.Achievement;

internal static class AchievementParser
{
    public static UIAF? Parse(byte[] bytes)
    {
        using CodedInputStream stream = new(bytes);
        List<Dictionary<uint, uint>> data = [];
        int errTimes = 0;
        try
        {
            uint tag;
            while ((tag = stream.ReadTag()) != 0)
            {
                if ((tag & 7) == 2)
                {
                    // is LengthDelimited
                    Dictionary<uint, uint>? dict = [];
                    using CodedInputStream eStream = stream.ReadLengthDelimitedAsStream();
                    try
                    {
                        while ((tag = eStream.ReadTag()) != 0)
                        {
                            if ((tag & 7) != 0)
                            {
                                // not VarInt
                                dict = null;
                                break;
                            }

                            dict[tag >> 3] = eStream.ReadUInt32();
                        }

                        if (dict != null)
                        {
                            data.Add(dict);
                        }
                    }
                    catch (InvalidProtocolBufferException)
                    {
                        if (errTimes++ > 0)
                        {
                            // allows 1 fail on 'reward_taken_goal_id_list'
                            throw;
                        }
                    }
                }
            }
        }
        catch (InvalidProtocolBufferException)
        {
        }

        if (data.Count is 0)
        {
            return default;
        }

        uint timestampId, statusId, id, currentId;
        if (data.Count > 20)
        {
            /* uwu */
            (timestampId, int cnt) = data //        ↓ 2020-09-15 04:15:14
                .GroupKeys(value => value > 1600114514).Select(g => (g.Key, g.Count())).MaxBy(p => p.Item2);
            statusId = data //           FINISHED ↓    ↓ REWARD_TAKEN
                .GroupKeys(value => value is 2 or 3).First(g => g.Count() == cnt).Key;
            id = data //                                 ↓ id: 8xxxx
                .GroupKeys(value => (value / 10000) % 10 == 8).MaxBy(g => g.Count())!.Key;
            (currentId, _) = data
                .Where(d => d[statusId] is 2 or 3)
                .Select(d => d.ToDictionary().RemoveValues(timestampId, statusId, id).ToArray())
                .Where(d => d.Length == 2 && d[0].Value != d[1].Value)
                .GroupBy(a => a[0].Value > a[1].Value ? (a[0].Key, a[1].Key) : (a[1].Key, a[0].Key))
                .Select(g => (FieldIds: g.Key, Count: g.Count()))
                .MaxBy(p => p.Count)
                .FieldIds;
        }
        else
        {
            return default;
        }

        return new()
        {
            Info = UIAFInfo.CreateForYaeLib(),
            List = data.Select(
                dict => new UIAFItem
                {
                    Id = dict[id],
                    Current = dict.GetValueOrDefault(currentId),
                    Status = (AchievementStatus)dict[statusId],
                    Timestamp = dict.GetValueOrDefault(timestampId),
                }).ToImmutableArray(),
        };
    }
}