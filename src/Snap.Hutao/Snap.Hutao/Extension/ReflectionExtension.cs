// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Reflection;

namespace Snap.Hutao.Extension;

/// <summary>
/// 反射扩展
/// </summary>
internal static class ReflectionExtension
{
    /// <summary>
    /// 在指定的成员中尝试获取标记的特性
    /// </summary>
    /// <typeparam name="TAttribute">特性的类型</typeparam>
    /// <param name="type">类型</param>
    /// <param name="attribute">获取的特性</param>
    /// <returns>是否获取成功</returns>
    public static bool TryGetAttribute<TAttribute>(this Type type, [NotNullWhen(true)] out TAttribute? attribute)
        where TAttribute : Attribute
    {
        attribute = type.GetCustomAttribute<TAttribute>();
        return attribute != null;
    }

    /// <summary>
    /// 检测类型是否实现接口
    /// </summary>
    /// <typeparam name="TInterface">被实现的类型</typeparam>
    /// <param name="type">被检测的类型/param>
    /// <returns>是否实现接口</returns>
    [SuppressMessage("", "SA1615")]
    public static bool Implement<TInterface>(this Type type)
    {
        return type.IsAssignableTo(typeof(TInterface));
    }

    /// <summary>
    /// 检查程序集是否标记了指定的特性
    /// </summary>
    /// <typeparam name="TAttribute">特性类型</typeparam>
    /// <param name="assembly">指定的程序集</param>
    /// <returns>是否标记了指定的特性</returns>
    public static bool HasAttribute<TAttribute>(this Assembly assembly)
        where TAttribute : Attribute
    {
        return assembly.GetCustomAttribute<TAttribute>() is not null;
    }

    /// <summary>
    /// 对程序集中的所有类型进行指定的操作
    /// </summary>
    /// <param name="assembly">指定的程序集</param>
    /// <param name="action">进行的操作</param>
    public static void ForEachType(this Assembly assembly, Action<Type> action)
    {
        foreach (Type type in assembly.GetTypes())
        {
            action.Invoke(type);
        }
    }

    /// <summary>
    /// 按字段名称设置值
    /// </summary>
    /// <param name="obj">对象</param>
    /// <param name="fieldName">字段名称</param>
    /// <param name="value">值</param>
    public static void SetPrivateFieldValueByName(this object obj, string fieldName, object? value)
    {
        FieldInfo? fieldInfo = obj.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
        fieldInfo?.SetValue(obj, value);
    }
}
