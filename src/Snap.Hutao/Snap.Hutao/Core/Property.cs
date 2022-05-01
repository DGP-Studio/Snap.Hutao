// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;

namespace Snap.Hutao.Core;

/// <summary>
/// 快速创建 <see cref="TOwner"/> 的 <see cref="DependencyProperty"/>
/// </summary>
/// <typeparam name="TOwner">所有者的类型</typeparam>
internal static class Property<TOwner>
{
    /// <summary>
    /// 注册依赖属性
    /// </summary>
    /// <typeparam name="TProperty">属性的类型</typeparam>
    /// <param name="name">属性名称</param>
    /// <returns>注册的依赖属性</returns>
    public static DependencyProperty Depend<TProperty>(string name)
    {
        return DependencyProperty.Register(name, typeof(TProperty), typeof(TOwner), new PropertyMetadata(default(TProperty)));
    }

    /// <summary>
    /// 注册依赖属性
    /// </summary>
    /// <typeparam name="TProperty">属性的类型</typeparam>
    /// <param name="name">属性名称</param>
    /// <param name="defaultValue">默认值</param>
    /// <returns>注册的依赖属性</returns>
    public static DependencyProperty Depend<TProperty>(string name, TProperty defaultValue)
    {
        return DependencyProperty.Register(name, typeof(TProperty), typeof(TOwner), new PropertyMetadata(defaultValue));
    }

    /// <summary>
    /// 注册依赖属性
    /// </summary>
    /// <typeparam name="TProperty">属性的类型</typeparam>
    /// <param name="name">属性名称</param>
    /// <param name="defaultValue">默认值</param>
    /// <param name="callback">属性更改回调</param>
    /// <returns>注册的依赖属性</returns>
    public static DependencyProperty Depend<TProperty>(string name, TProperty defaultValue, PropertyChangedCallback callback)
    {
        return DependencyProperty.Register(name, typeof(TProperty), typeof(TOwner), new PropertyMetadata(defaultValue, callback));
    }

    /// <summary>
    /// 注册附加属性
    /// </summary>
    /// <typeparam name="TProperty">属性的类型</typeparam>
    /// <param name="name">属性名称</param>
    /// <returns>注册的附加属性</returns>
    public static DependencyProperty Attach<TProperty>(string name)
    {
        return DependencyProperty.RegisterAttached(name, typeof(TProperty), typeof(TOwner), new PropertyMetadata(default(TProperty)));
    }

    /// <summary>
    /// 注册附加属性
    /// </summary>
    /// <typeparam name="TProperty">属性的类型</typeparam>
    /// <param name="name">属性名称</param>
    /// <param name="defaultValue">默认值</param>
    /// <returns>注册的附加属性</returns>
    public static DependencyProperty Attach<TProperty>(string name, TProperty defaultValue)
    {
        return DependencyProperty.RegisterAttached(name, typeof(TProperty), typeof(TOwner), new PropertyMetadata(defaultValue));
    }

    /// <summary>
    /// 注册附加属性
    /// </summary>
    /// <typeparam name="TProperty">属性的类型</typeparam>
    /// <param name="name">属性名称</param>
    /// <param name="callback">属性更改回调</param>
    /// <returns>注册的附加属性</returns>
    public static DependencyProperty Attach<TProperty>(string name, PropertyChangedCallback callback)
    {
        return DependencyProperty.RegisterAttached(name, typeof(TProperty), typeof(TOwner), new PropertyMetadata(callback));
    }
}
