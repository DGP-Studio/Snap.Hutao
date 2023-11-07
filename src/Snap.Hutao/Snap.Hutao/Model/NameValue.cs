// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Model;

/// <summary>
/// 封装带有名称描述的值
/// 在绑定枚举变量时非常有用
/// https://github.com/microsoft/microsoft-ui-xaml/issues/4266
/// 直接绑定枚举变量会显示 Windows.Foundation.IReference{T}
/// </summary>
/// <typeparam name="T">包含值的类型</typeparam>
[HighQuality]
internal sealed class NameValue<T>
{
    /// <summary>
    /// 构造一个新的命名的值
    /// </summary>
    /// <param name="name">命名</param>
    /// <param name="value">值</param>
    public NameValue(string name, T value)
    {
        Name = name;
        Value = value;
    }

    /// <summary>
    /// 名称
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// 值
    /// </summary>
    public T Value { get; }
}