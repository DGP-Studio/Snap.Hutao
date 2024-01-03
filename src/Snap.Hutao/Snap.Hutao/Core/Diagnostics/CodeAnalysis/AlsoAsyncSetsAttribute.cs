// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Core.Diagnostics.CodeAnalysis;

/// <summary>
/// 指示此特性附加的属性会在属性改变后会异步地设置的其他属性
/// </summary>
[AttributeUsage(AttributeTargets.Property, Inherited = false)]
internal sealed class AlsoAsyncSetsAttribute : Attribute
{
    public AlsoAsyncSetsAttribute(string propertyName)
    {
    }

    public AlsoAsyncSetsAttribute(string propertyName1, string propertyName2)
    {
    }

    public AlsoAsyncSetsAttribute(params string[] propertyNames)
    {
    }
}