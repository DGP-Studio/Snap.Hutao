// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Binding.Gacha;
using Snap.Hutao.Model.Metadata.Abstraction;
using Snap.Hutao.Model.Metadata.Avatar;
using Snap.Hutao.Model.Metadata.Weapon;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Snap.Hutao.Service.GachaLog.Factory;

/// <summary>
/// 统计拓展
/// </summary>
public static class GachaStatisticsExtensions
{
    /// <summary>
    /// 值域压缩
    /// </summary>
    /// <param name="b">源</param>
    /// <returns>压缩值</returns>
    public static byte HalfRange(this byte b)
    {
        // [0,256] -> [0,128]-> [64,172]
        return (byte)((b / 2) + 64);
    }

    /// <summary>
    /// 求平均值
    /// </summary>
    /// <param name="span">跨度</param>
    /// <returns>平均值</returns>
    public static byte Average(this Span<byte> span)
    {
        int sum = 0;
        int count = 0;
        foreach (byte b in span)
        {
            sum += b;
            count++;
        }

        return unchecked((byte)(sum / count));
    }

    /// <summary>
    /// 增加计数
    /// </summary>
    /// <typeparam name="TKey">键类型</typeparam>
    /// <param name="dict">字典</param>
    /// <param name="key">键</param>
    public static void Increase<TKey>(this Dictionary<TKey, int> dict, TKey key)
        where TKey : notnull
    {
        ++CollectionsMarshal.GetValueRefOrAddDefault(dict, key, out _);
    }

    /// <summary>
    /// 增加计数
    /// </summary>
    /// <typeparam name="TKey">键类型</typeparam>
    /// <param name="dict">字典</param>
    /// <param name="key">键</param>
    /// <returns>是否存在键值</returns>
    public static bool TryIncrease<TKey>(this Dictionary<TKey, int> dict, TKey key)
        where TKey : notnull
    {
        ref int value = ref CollectionsMarshal.GetValueRefOrNullRef(dict, key);
        if (!Unsafe.IsNullRef(ref value))
        {
            ++value;
            return true;
        }

        return false;
    }

    /// <summary>
    /// 将计数器转换为统计物品列表
    /// </summary>
    /// <param name="dict">计数器</param>
    /// <returns>统计物品列表</returns>
    public static List<StatisticsItem> ToStatisticsList(this Dictionary<IStatisticsItemSource, int> dict)
    {
        return dict.Select(kvp => kvp.Key.ToStatisticsItem(kvp.Value)).ToList();
    }

    /// <summary>
    /// 将计数器转换为统计物品列表
    /// </summary>
    /// <param name="dict">计数器</param>
    /// <returns>统计物品列表</returns>
    public static List<StatisticsItem> ToStatisticsList(this Dictionary<Avatar, int> dict)
    {
        return dict.Select(kvp => kvp.Key.ToStatisticsItem(kvp.Value)).ToList();
    }

    /// <summary>
    /// 将计数器转换为统计物品列表
    /// </summary>
    /// <param name="dict">计数器</param>
    /// <returns>统计物品列表</returns>
    public static List<StatisticsItem> ToStatisticsList(this Dictionary<Weapon, int> dict)
    {
        return dict.Select(kvp => kvp.Key.ToStatisticsItem(kvp.Value)).ToList();
    }
}
