// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Google.Protobuf;
using Snap.Hutao.Core.Protobuf;
using Snap.Hutao.Model.InterChange.Achievement;
using Snap.Hutao.Model.Intrinsic;

namespace Snap.Hutao.Service.Yae.Achievement;

internal static class AchievementParser
{
    public static UIAF? Parse(ByteString bytes, AchievementFieldId? fieldId = default)
    {
        using (CodedInputStream stream = bytes.CreateCodedInput())
        {
            List<Dictionary<int, uint>> dataList = [];
            int errorTimes = 0;
            try
            {
                while (stream.TryReadTag(out uint tag))
                {
                    if (WireFormat.GetTagWireType(tag) is not WireFormat.WireType.LengthDelimited)
                    {
                        continue;
                    }

                    Dictionary<int, uint>? varInts = [];
                    using (CodedInputStream inputStream = stream.UnsafeReadLengthDelimitedStream())
                    {
                        try
                        {
                            while (inputStream.TryReadTag(out uint tag2))
                            {
                                if (WireFormat.GetTagWireType(tag2) is not WireFormat.WireType.Varint)
                                {
                                    varInts = default;
                                    break;
                                }

                                varInts[WireFormat.GetTagFieldNumber(tag2)] = inputStream.ReadUInt32();
                            }

                            if (varInts is { Count: > 2 })
                            {
                                dataList.Add(varInts);
                            }
                        }
                        catch (InvalidProtocolBufferException)
                        {
                            if (errorTimes++ > 0)
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

            if (dataList.Count > 20)
            {
                FieldConverter converter;
                if (fieldId is not null)
                {
                    converter = new(fieldId.Id, fieldId.CurrentProgress, fieldId.Status, fieldId.FinishTimestamp);
                }
                else
                {
                    // :                                                                   ↓ 2020-09-15 04:15:14
                    (int timestampField, int count) = dataList.CountByKey(value => value > 1600114514).MaxBy(kvp => kvp.Value);

                    // :                                           FINISHED ↓     ↓ REWARD_TAKEN
                    int statusField = dataList.CountByKey(value => value is 2U or 3U).First(kvp => kvp.Value == count).Key;

                    // :                                                  ↓ id: 8xxxx
                    int achievementIdField = dataList.CountByKey(value => (value / 10000) % 10 == 8).MaxBy(kvp => kvp.Value).Key;

                    HashSet<int> excludedFields = [timestampField, statusField, achievementIdField];

                    (int currentField, _) = dataList
                        .Where(data => data[statusField] is 2U or 3U)
                        .Select(data => data.WithKeysRemoved(excludedFields).ToArray())
                        .Where(data => data.Length == 2 && data[0].Value != data[1].Value)
                        .CountBy(a => a[0].Value > a[1].Value ? (a[0].Key, a[1].Key) : (a[1].Key, a[0].Key))
                        .MaxBy(p => p.Value)
                        .Key;

                    converter = new(achievementIdField, currentField, statusField, timestampField);
                }

                return new()
                {
                    Info = UIAFInfo.CreateForEmbeddedYae(),
                    List = [.. dataList.Select(converter.Convert)],
                };
            }

            return default;
        }
    }

    private sealed class FieldConverter
    {
        private readonly int achievementIdField;
        private readonly int currentField;
        private readonly int statusField;
        private readonly int timestampField;

        public FieldConverter(int achievementIdField, int currentField, int statusField, int timestampField)
        {
            this.achievementIdField = achievementIdField;
            this.currentField = currentField;
            this.statusField = statusField;
            this.timestampField = timestampField;
        }

        public UIAFItem Convert(Dictionary<int, uint> dict)
        {
            return new()
            {
                Id = dict[achievementIdField],
                Current = dict.GetValueOrDefault(currentField),
                Status = (AchievementStatus)dict[statusField],
                Timestamp = dict.GetValueOrDefault(timestampField),
            };
        }
    }
}