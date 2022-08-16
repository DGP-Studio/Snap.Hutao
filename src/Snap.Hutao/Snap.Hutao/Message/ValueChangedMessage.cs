// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Message;

/// <summary>
/// 值变化消息
/// </summary>
/// <typeparam name="TValue">值的类型</typeparam>
internal abstract class ValueChangedMessage<TValue>
    where TValue : class
{
    /// <summary>
    /// 构造一个新的值变化消息
    /// </summary>
    /// <param name="oldValue">旧值</param>
    /// <param name="newValue">新值</param>
    public ValueChangedMessage(TValue? oldValue, TValue? newValue)
    {
        OldValue = oldValue;
        NewValue = newValue;
    }

    /// <summary>
    /// 旧的值
    /// </summary>
    public TValue? OldValue { get; private set; }

    /// <summary>
    /// 新的值
    /// </summary>
    public TValue? NewValue { get; private set; }
}