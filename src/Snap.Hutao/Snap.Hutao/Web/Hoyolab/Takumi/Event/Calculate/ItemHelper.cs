// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Runtime.InteropServices;

namespace Snap.Hutao.Web.Hoyolab.Takumi.Event.Calculate;

internal static class ItemHelper
{
    public static List<Item> Merge(List<Item>? left, List<Item>? right)
    {
        return (left, right) switch
        {
            ([_, ..], [_, ..]) => MergeNotEmpty(left, right),
            ([_, ..], null or []) => left,
            (null or [], [_, ..]) => right,
            _ => [],
        };
    }

    private static List<Item> MergeNotEmpty(List<Item> left, List<Item> right)
    {
        List<Item> result = new(left.Count + right.Count);
        result.AddRange(left);

        foreach (ref readonly Item item in CollectionsMarshal.AsSpan(right))
        {
            uint id = item.Id;
            if (result.SingleOrDefault(i => i.Id == id) is { } existed)
            {
                existed.Num += item.Num;
            }
            else
            {
                result.Add(item);
            }
        }

        return result;
    }
}