// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Bridge;

/// <summary>
/// Web 调用
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
public class JsMethodAttribute : Attribute
{
    /// <summary>
    /// 构造一个新的Web 调用特性
    /// </summary>
    /// <param name="name">函数名称</param>
    public JsMethodAttribute(string name)
    {
        Name = name;
    }

    /// <summary>
    /// 调用函数名称
    /// </summary>
    public string Name { get; init; }
}