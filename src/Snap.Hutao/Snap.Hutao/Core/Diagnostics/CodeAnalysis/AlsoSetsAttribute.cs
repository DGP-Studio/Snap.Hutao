// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Core.Diagnostics.CodeAnalysis;

/// <summary>
/// 指示此特性附加的属性会在属性改变后会设置的其他属性
/// </summary>
[Obsolete]
[AttributeUsage(AttributeTargets.Property, Inherited = false)]
internal sealed class AlsoSetsAttribute : Attribute
{
    public AlsoSetsAttribute(string propertyName)
    {
    }

    public AlsoSetsAttribute(string propertyName1, string propertyName2)
    {
    }

    public AlsoSetsAttribute(params string[] propertyNames)
    {
    }
}