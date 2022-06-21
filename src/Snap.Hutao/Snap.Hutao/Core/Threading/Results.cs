// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Core.Threading;

/// <summary>
/// 构造复杂的结果
/// </summary>
public static class Results
{
    /// <summary>
    /// 根据条件构造结果
    /// </summary>
    /// <typeparam name="T">结果的类型</typeparam>
    /// <param name="condition">条件</param>
    /// <param name="trueValue">条件符合时的值</param>
    /// <param name="falseValue">条件不符合时的值</param>
    /// <returns>结果</returns>
    public static Result<bool, T> Condition<T>(bool condition, T trueValue, T falseValue)
        where T : notnull
    {
        return new(condition, condition ? trueValue : falseValue);
    }
}