// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Markup;

namespace Snap.Hutao.Control.Markup;

[MarkupExtensionReturnType(ReturnType = typeof(uint))]
internal sealed class UInt32Extension : MarkupExtension
{
    public string Value { get; set; } = default!;

    protected override object ProvideValue()
    {
        _ = uint.TryParse(Value, out uint result);
        return result;
    }
}