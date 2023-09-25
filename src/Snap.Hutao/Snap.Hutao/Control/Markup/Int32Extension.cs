// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Markup;

namespace Snap.Hutao.Control.Markup;

[MarkupExtensionReturnType(ReturnType = typeof(int))]
internal sealed class Int32Extension : MarkupExtension
{
    public string Value { get; set; } = default!;

    protected override object ProvideValue()
    {
        return XamlBindingHelper.ConvertValue(typeof(int), Value);
    }
}