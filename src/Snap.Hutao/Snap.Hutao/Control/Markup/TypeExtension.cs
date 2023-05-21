// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Markup;

namespace Snap.Hutao.Control.Markup;

/// <summary>
/// 类型拓展
/// </summary>
[MarkupExtensionReturnType(ReturnType = typeof(Type))]
internal sealed class TypeExtension : MarkupExtension
{
    /// <summary>
    /// 类型
    /// </summary>
    public Type Type { get; set; } = default!;

    /// <inheritdoc/>
    protected override object ProvideValue()
    {
        return Type;
    }
}