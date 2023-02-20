// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Core.DependencyInjection.Annotation;

/// <summary>
/// 指示被标注的类型可注入
/// 由源生成器生成注入代码
/// </summary>
[HighQuality]
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
internal sealed class InjectionAttribute : Attribute
{
    /// <summary>
    /// 指示该类将注入为不带有接口实现的类
    /// </summary>
    /// <param name="injectAs">指示注入方法</param>
    public InjectionAttribute(InjectAs injectAs)
    {
    }

    /// <summary>
    /// 指示该类将注入为带有接口实现的类
    /// </summary>
    /// <param name="injectAs">指示注入方法</param>
    /// <param name="interfaceType">实现的接口类型</param>
    public InjectionAttribute(InjectAs injectAs, Type interfaceType)
    {
    }
}