// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Metadata.Abstraction;
using Snap.Hutao.ViewModel.GachaLog;
using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using Windows.UI;

namespace Snap.Hutao.Service.GachaLog.Factory;

internal static class GachaStatisticsExtension
{
    private static readonly ConcurrentDictionary<string, Color> KnownColors = [];

    public static void CompleteAdding(this List<SummaryItem> summaryItems, int guaranteeOrangeThreshold)
    {
        // We can't trust first item's prev state.
        bool isPreviousUp = true;

        // Mark the IsGuarantee
        foreach (ref readonly SummaryItem item in CollectionsMarshal.AsSpan(summaryItems))
        {
            if (item.IsUp && !isPreviousUp)
            {
                item.IsGuarantee = true;
            }

            isPreviousUp = item.IsUp;
            item.Color = GetUniqueColorByName(item.Name);
            item.GuaranteeOrangeThreshold = guaranteeOrangeThreshold;
        }

        // Reverse items
        summaryItems.Reverse();
    }

    public static ImmutableArray<StatisticsItem> ToStatisticsImmutableArray<TItem>(this Dictionary<TItem, int> countDict)
        where TItem : IStatisticsItemConvertible
    {
        return [.. countDict.Select(kvp => kvp.Key.ToStatisticsItem(kvp.Value)).OrderByDescending(item => item.Count)];
    }

    [SuppressMessage("", "IDE0057")]
    private static Color GetUniqueColorByName(string name)
    {
        if (KnownColors.TryGetValue(name, out Color color))
        {
            return color;
        }

        ReadOnlySpan<byte> codes = MD5.HashData(Encoding.UTF8.GetBytes(name));

        // ReSharper disable once ReplaceSliceWithRangeIndexer
        Color current = Color.FromArgb(255, codes.Slice(0, 5).Average(), codes.Slice(5, 5).Average(), codes.Slice(10, 5).Average());
        KnownColors.TryAdd(name, current);
        return current;
    }
}