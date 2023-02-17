// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Model.Binding.AvatarProperty;

/// <summary>
/// 角色属性值
/// </summary>
[HighQuality]
internal sealed class AvatarProperty
{
    /// <summary>
    /// 构造一个新的角色属性值
    /// </summary>
    /// <param name="name">名称</param>
    /// <param name="value">白字</param>
    /// <param name="addValue">绿字</param>
    public AvatarProperty(string name, string value, string? addValue = null)
    {
        Name = name;
        Value = value;
        AddValue = addValue;
    }

    /// <summary>
    /// 名称
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// 白字
    /// </summary>
    public string Value { get; }

    /// <summary>
    /// 绿字
    /// </summary>
    public string? AddValue { get; }
}