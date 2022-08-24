// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Controls;
using Snap.Hutao.Extension;

namespace Snap.Hutao.Control.Extension;

/// <summary>
/// 对话框扩展
/// </summary>
internal static class ContentDialogExtensions
{
    /// <summary>
    /// 阻止用户交互
    /// </summary>
    /// <param name="contentDialog">对话框</param>
    /// <returns>用于恢复用户交互</returns>
    public static IDisposable BlockInteraction(this ContentDialog contentDialog)
    {
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