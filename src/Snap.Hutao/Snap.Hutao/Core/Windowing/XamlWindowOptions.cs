// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Windows.Graphics;

namespace Snap.Hutao.Core.Windowing;

/// <summary>
/// Window 选项
/// </summary>
internal sealed class XamlWindowOptions
{
    public XamlWindowOptions(Window window, FrameworkElement titleBar, SizeInt32 initSize, string? persistSize = default)
    {
        PersistRectKey = persistSize;
    }

    public SizeInt32 InitSize { get; }

    public string? PersistRectKey { get; }
}