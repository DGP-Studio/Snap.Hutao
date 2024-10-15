// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Frozen;
using System.Text;

namespace Snap.Hutao.Core;

/// <summary>
/// 类型名称帮助类
/// Directly copied from .NET Runtime library
/// </summary>
[SuppressMessage("", "SH007")]
internal static class TypeNameHelper
{
    private const char DefaultNestedTypeDelimiter = '+';

    private static readonly FrozenDictionary<Type, string> BuiltInTypeNames = FrozenDictionary.ToFrozenDictionary(
    [
        KeyValuePair.Create(typeof(void), "void"),
        KeyValuePair.Create(typeof(bool), "bool"),
        KeyValuePair.Create(typeof(byte), "byte"),
        KeyValuePair.Create(typeof(char), "char"),
        KeyValuePair.Create(typeof(decimal), "decimal"),
        KeyValuePair.Create(typeof(double), "double"),
        KeyValuePair.Create(typeof(float), "float"),
        KeyValuePair.Create(typeof(int), "int"),
        KeyValuePair.Create(typeof(long), "long"),
        KeyValuePair.Create(typeof(object), "object"),
        KeyValuePair.Create(typeof(sbyte), "sbyte"),
        KeyValuePair.Create(typeof(short), "short"),
        KeyValuePair.Create(typeof(string), "string"),
        KeyValuePair.Create(typeof(uint), "uint"),
        KeyValuePair.Create(typeof(ulong), "ulong"),
        KeyValuePair.Create(typeof(ushort), "ushort"),
    ]);

    /// <summary>
    /// 获取对象类型的显示名称
    /// </summary>
    /// <param name="item">物品</param>
    /// <param name="fullName">是否全名</param>
    /// <returns>对象类型的显示名称</returns>
    [return: NotNullIfNotNull(nameof(item))]
    public static string? GetTypeDisplayName(object? item, bool fullName = true)
    {
        return item is null ? null : GetTypeDisplayName(item.GetType(), fullName);
    }

    /// <summary>
    /// Pretty print a type name.
    /// </summary>
    /// <param name="type">The <see cref="Type"/>.</param>
    /// <param name="fullName"><c>true</c> to print a fully qualified name.</param>
    /// <param name="includeGenericParameterNames"><c>true</c> to include generic parameter names.</param>
    /// <param name="includeGenericParameters"><c>true</c> to include generic parameters.</param>
    /// <param name="nestedTypeDelimiter">Character to use as a delimiter in nested type names</param>
    /// <returns>The pretty printed type name.</returns>
    public static string GetTypeDisplayName(Type type, bool fullName = true, bool includeGenericParameterNames = false, bool includeGenericParameters = true, char nestedTypeDelimiter = DefaultNestedTypeDelimiter)
    {
        StringBuilder? builder = null;
        string? name = ProcessType(ref builder, type, new(fullName, includeGenericParameterNames, includeGenericParameters, nestedTypeDelimiter));
        return name ?? builder?.ToString() ?? string.Empty;
    }

    private static string? ProcessType(ref StringBuilder? builder, Type type, in DisplayNameOptions options)
    {
        if (type.IsGenericType)
        {
            Type[] genericArguments = type.GetGenericArguments();
            builder ??= new StringBuilder();
            ProcessGenericType(builder, type, genericArguments, genericArguments.Length, options);
        }
        else if (type.IsArray)
        {
            builder ??= new StringBuilder();
            ProcessArrayType(builder, type, options);
        }
        else if (BuiltInTypeNames.TryGetValue(type, out string? builtInName))
        {
            if (builder is null)
            {
                return builtInName;
            }

            builder.Append(builtInName);
        }
        else if (type.IsGenericParameter)
        {
            if (options.IncludeGenericParameterNames)
            {
                if (builder is null)
                {
                    return type.Name;
                }

                builder.Append(type.Name);
            }
        }
        else
        {
            string name = options.FullName ? type.FullName! : type.Name;

            if (builder is null)
            {
                return options.NestedTypeDelimiter is DefaultNestedTypeDelimiter ? name : name.Replace(DefaultNestedTypeDelimiter, options.NestedTypeDelimiter);
            }

            builder.Append(name);
            if (options.NestedTypeDelimiter is not DefaultNestedTypeDelimiter)
            {
                builder.Replace(DefaultNestedTypeDelimiter, options.NestedTypeDelimiter, builder.Length - name.Length, name.Length);
            }
        }

        return null;
    }

    private static void ProcessArrayType(StringBuilder builder, Type type, in DisplayNameOptions options)
    {
        Type innerType = type;
        while (innerType.IsArray)
        {
            innerType = innerType.GetElementType()!;
        }

        ProcessType(ref builder!, innerType, options);

        while (type.IsArray)
        {
            builder.Append('[');
            builder.Append(',', type.GetArrayRank() - 1);
            builder.Append(']');
            type = type.GetElementType()!;
        }
    }

    private static void ProcessGenericType(StringBuilder builder, Type type, Type[] genericArguments, int length, in DisplayNameOptions options)
    {
        int offset = 0;
        if (type.IsNested)
        {
            offset = type.DeclaringType!.GetGenericArguments().Length;
        }

        if (options.FullName)
        {
            if (type.IsNested)
            {
                ProcessGenericType(builder, type.DeclaringType!, genericArguments, offset, options);
                builder.Append(options.NestedTypeDelimiter);
            }
            else if (!string.IsNullOrEmpty(type.Namespace))
            {
                builder.Append(type.Namespace);
                builder.Append('.');
            }
        }

        int genericPartIndex = type.Name.AsSpan().IndexOf('`');
        if (genericPartIndex <= 0)
        {
            builder.Append(type.Name);
            return;
        }

        builder.Append(type.Name, 0, genericPartIndex);

        if (options.IncludeGenericParameters)
        {
            builder.Append('<');
            for (int i = offset; i < length; i++)
            {
                ProcessType(ref builder!, genericArguments[i], options);
                if (i + 1 == length)
                {
                    continue;
                }

                builder.Append(',');
                if (options.IncludeGenericParameterNames || !genericArguments[i + 1].IsGenericParameter)
                {
                    builder.Append(' ');
                }
            }

            builder.Append('>');
        }
    }

    private readonly struct DisplayNameOptions
    {
        public DisplayNameOptions(bool fullName, bool includeGenericParameterNames, bool includeGenericParameters, char nestedTypeDelimiter)
        {
            FullName = fullName;
            IncludeGenericParameters = includeGenericParameters;
            IncludeGenericParameterNames = includeGenericParameterNames;
            NestedTypeDelimiter = nestedTypeDelimiter;
        }

        public bool FullName { get; }

        public bool IncludeGenericParameters { get; }

        public bool IncludeGenericParameterNames { get; }

        public char NestedTypeDelimiter { get; }
    }
}