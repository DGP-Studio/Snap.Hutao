// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Markup;

namespace Snap.Hutao.Control.Markup;

/// <summary>
/// Uri扩展
/// </summary>
[MarkupExtensionReturnType(ReturnType = typeof(Uri))]
public sealed class UriExtension : MarkupExtension
{
    /// <summary>
    /// 构造一个新的Uri扩展
    /// </summary>
    public UriExtension()
    {
    }

    /// <summary>
    /// 地址
    /// </summary>
    public string? Value { get; set; }

    /// <inheritdoc/>
    protected override object ProvideValue()
    {
        return new Uri(Value ?? string.Empty);
    }
}