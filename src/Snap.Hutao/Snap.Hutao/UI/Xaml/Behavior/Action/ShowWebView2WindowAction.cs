// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Microsoft.Xaml.Interactivity;
using Snap.Hutao.UI.Xaml.View.Window.WebView2;

namespace Snap.Hutao.UI.Xaml.Behavior.Action;

[DependencyProperty("ContentProvider", typeof(IWebView2ContentProvider))]
internal sealed partial class ShowWebView2WindowAction : DependencyObject, IAction
{
    public object? Execute(object sender, object parameter)
    {
        WebView2Window window = new(((FrameworkElement)sender).XamlRoot.ContentIslandEnvironment.AppWindowId, ContentProvider);
        window.Activate();
        return default!;
    }
}