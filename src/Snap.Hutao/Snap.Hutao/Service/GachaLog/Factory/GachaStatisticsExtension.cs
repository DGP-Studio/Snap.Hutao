// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Metadata.Abstraction;
using Snap.Hutao.ViewModel.GachaLog;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using Windows.UI;

namespace Snap.Hutao.Service.GachaLog.Factory;

/// <summary>
/// 统计拓展
/// </summary>
internal static class GachaStatisticsExtension
{
    /// <summary>
    /// 完成添加
    /// </summary>
    /// <param name="summaryItems">简述物品列表</param>
    /// <param name="guaranteeOrangeThreshold">五星保底阈值</param>
    public static void CompleteAdding(this List<SummaryItem> summaryItems, int guaranteeOrangeThreshold)
    {
        // we can't trust first item's prev state.
        bool isPreviousUp = true;

        // mark the IsGuarantee
        foreach (ref readonly SummaryItem item in CollectionsMarshal.AsSpan(summaryItems))
        {
            if (item.IsUp && (!isPreviousUp))
            {
                item.IsGuarantee = true;
            }

            isPreviousUp = item.IsUp;
            item.Color = GetColorByName(item.Name);
            item.GuaranteeOrangeThreshold = guaranteeOrangeThreshold;
        }

        // reverse items
        summaryItems.Reverse();
    }

    /// <summary>
    /// 将计数器转换为统计物品列表
    /// </summary>
    /// <typeparam name="TItem">物品类型</typeparam>
    /// <param name="dict">计数器</param>
    /// <returns>统计物品列表</returns>
    public static List<StatisticsItem> ToStatisticsList<TItem>(this Dictionary<TItem, int> dict)
        where TItem : IStatisticsItemConvertible
    {
        IOrderedEnumerable<StatisticsItem> result = dict
            .Select(kvp => kvp.Key.ToStatisticsItem(kvp.Value))
            .OrderByDescending(item => item.Count);
        return [.. result];
    }

    [SuppressMessage("", "IDE0057")]
    private static Color GetColorByName(string name)
    {
        ReadOnlySpan<byte> codes = MD5.HashData(Encoding.UTF8.GetBytes(name));
        return Color.FromArgb(255, codes[..5].Average(), codes.Slice(5, 5).Average(), codes.Slice(10, 5).Average());
    }
}