// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Runtime.InteropServices;

namespace Snap.Hutao.Web.Hoyolab.Takumi.Event.Calculate;

[HighQuality]
internal static class ItemHelper
{
    public static List<Item> Merge(List<Item>? left, List<Item>? right)
    {
        if (left.IsNullOrEmpty() && right.IsNullOrEmpty())
        {
            return [];
        }

        if (left.IsNullOrEmpty() && !right.IsNullOrEmpty())
        {
            return right;
        }

        if (right.IsNullOrEmpty() && !left.IsNullOrEmpty())
        {
            return left;
        }

        ArgumentNullException.ThrowIfNull(left);
        ArgumentNullException.ThrowIfNull(right);

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