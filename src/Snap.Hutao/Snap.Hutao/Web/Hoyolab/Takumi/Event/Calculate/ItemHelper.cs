// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Immutable;
using System.Diagnostics;

namespace Snap.Hutao.Web.Hoyolab.Takumi.Event.Calculate;

internal static class ItemHelper
{
    public static ImmutableArray<Item> Merge(ImmutableArray<Item> left, ImmutableArray<Item> right)
    {
        Debug.Assert(!left.IsDefault);
        Debug.Assert(!right.IsDefault);

        return (left, right) switch
        {
            ([_, ..], [_, ..]) => MergeNotEmpty(left, right),
            ([_, ..], []) => left,
            ([], [_, ..]) => right,
            _ => [],
        };
    }

    private static ImmutableArray<Item> MergeNotEmpty(ImmutableArray<Item> left, ImmutableArray<Item> right)
    {
        ImmutableArray<Item>.Builder result = ImmutableArray.CreateBuilder<Item>(left.Length + right.Length);
        result.AddRange(left);

        foreach (ref readonly Item item in right.AsSpan())
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

        return result.ToImmutable();
    }
}