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
internal sealed class NameValue<T>
{
    public NameValue(string name, T value)
    {
        Name = name;
        Value = value;
    }

    public string Name { get; }

    public T Value { get; }
}