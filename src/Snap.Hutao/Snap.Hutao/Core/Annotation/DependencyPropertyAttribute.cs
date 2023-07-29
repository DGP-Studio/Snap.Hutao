// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Core.Annotation;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
internal sealed class DependencyPropertyAttribute : Attribute
{
    public DependencyPropertyAttribute(string name, Type type)
    {
    }

    public DependencyPropertyAttribute(string name, Type type, object defaultValue)
    {
    }

    public DependencyPropertyAttribute(string name, Type type, object defaultValue, string valueChangedCallbackName)
    {
    }

    public bool IsAttached { get; set; }

    public Type AttachedType { get; set; } = default!;
}