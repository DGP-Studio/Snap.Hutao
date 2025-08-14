// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;

namespace Snap.Hutao.UI.Xaml;

[DependencyProperty("DataContext", typeof(object))]
[Test<bool>("Test", 42)]
internal sealed partial class BindingProxy : DependencyObject;

internal sealed class TestAttribute<T> : Attribute
{
    public TestAttribute(string name, int value)
    {
        Name = name;
        Value = value;
    }

    public string Name { get; }

    public int Value { get; }
}