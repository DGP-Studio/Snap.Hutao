// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Xaml.Interactivity;
using Snap.Hutao.Control.Behavior;
using Snap.Hutao.Core.Threading;
using Snap.Hutao.Extension;

namespace Snap.Hutao.Control.Extension;

/// <summary>
/// 对话框扩展
/// </summary>
internal static class ContentDialogExtensions
{
    /// <summary>
    /// 针对窗口进行初始化
    /// </summary>
    /// <param name="contentDialog">对话框</param>
    /// <param name="window">窗口</param>
    /// <returns>初始化完成的对话框</returns>
    public static ContentDialog InitializeWithWindow(this ContentDialog contentDialog, Window window)
    {
        contentDialog.XamlRoot = window.Content.XamlRoot;
        Interaction.SetBehaviors(contentDialog, new() { new ContentDialogBehavior() });

        return contentDialog;
    }

    /// <summary>
    /// 阻止用户交互
    /// </summary>
    /// <param name="contentDialog">对话框</param>
    /// <param name="switchToMainThread">切换到主线程</param>
    /// <returns>用于恢复用户交互</returns>
    public static async ValueTask<IDisposable> BlockAsync(this ContentDialog contentDialog, bool switchToMainThread = false)
    {
        if (switchToMainThread)
        {
            await ThreadHelper.SwitchToMainThreadAsync();
        }

        contentDialog.ShowAsync().AsTask().SafeForget();
        return new ContentDialogHider(contentDialog);
    }

    private struct ContentDialogHider : IDisposable
    {
        private readonly ContentDialog contentDialog;

        public ContentDialogHider(ContentDialog contentDialog)
        {
            this.contentDialog = contentDialog;
        }

        public void Dispose()
        {
            contentDialog.Hide();
        }
    }
}