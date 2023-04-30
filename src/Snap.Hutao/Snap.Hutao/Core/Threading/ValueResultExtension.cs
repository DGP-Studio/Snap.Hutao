// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Core.Threading;

/// <summary>
/// 结果扩展
/// </summary>
internal static class ValueResultExtension
{
    /// <summary>
    /// 尝试获取结果的值
    /// </summary>
    /// <typeparam name="TValue">值的类型</typeparam>
    /// <param name="valueResult">结果</param>
    /// <param name="value">值</param>
    /// <returns>是否获取成功</returns>
    public static bool TryGetValue<TValue>(this in ValueResult<bool, TValue> valueResult,[NotNullWhen(true)] out TValue value)
    {
        value = valueResult.Value;
        return valueResult.IsOk;
    }
}