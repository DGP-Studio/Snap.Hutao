// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Xaml.Interactivity;
using Snap.Hutao.Control.Behavior;

namespace Snap.Hutao.Control;

/// <summary>
/// 继承自 <see cref="ContentDialog"/> 实现了某些便捷功能
/// </summary>
internal class ContentDialog2 : ContentDialog
{
    /// <summary>
    /// 构造一个新的对话框
    /// </summary>
    /// <param name="window">窗口</param>
    public ContentDialog2(Window window)
    {
        DefaultStyleKey = typeof(ContentDialog);
        XamlRoot = window.Content.XamlRoot;
        Interaction.SetBehaviors(this, new() { new ContentDialogBehavior() });
    }
}
