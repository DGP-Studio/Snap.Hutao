// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Core.DependencyInjection.Annotation;

/// <summary>
/// 指示被标注的类型可注入
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public class InjectionAttribute : Attribute
{
    /// <summary>
    /// 指示该类将注入为不带有接口实现的类
    /// </summary>
    /// <param name="injectAs">指示注入方法</param>
    public InjectionAttribute(InjectAs injectAs)
    {
        InjectAs = injectAs;
    }

    /// <summary>
    /// 指示该类将注入为带有接口实现的类
    /// </summary>
    /// <param name="injectAs">指示注入方法</param>
    /// <param name="impl">实现的接口类型</param>
    public InjectionAttribute(InjectAs injectAs, Type impl)
    {
        InterfaceType = impl;
        InjectAs = injectAs;
    }

    /// <summary>
    /// 注入类型
    /// </summary>
    public InjectAs InjectAs { get; }

    /// <summary>
    /// 该类实现的接口类型
    /// </summary>
    public Type? InterfaceType { get; }
}
