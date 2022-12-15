// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.Takumi.Event.Calculate;

/// <summary>
/// 物品帮助类
/// </summary>
public static class ItemHelper
{
    /// <summary>
    /// 合并两个物品列表
    /// </summary>
    /// <param name="left">左列表</param>
    /// <param name="right">右列表</param>
    /// <returns>合并且排序好的列表</returns>
    public static List<Item> Merge(List<Item>? left, List<Item>? right)
    {
        if (left == null && right == null)
        {
            return new(0);
        }

        if (right == null)
        {
            return left!;
        }

        if (left == null)
        {
            return right!;
        }

        List<Item> result = new(left.Count + right.Count);
        result.AddRange(left);

        foreach (Item item in right)
        {
            if (result.SingleOrDefault(i => i.Id == item.Id) is Item existed)
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