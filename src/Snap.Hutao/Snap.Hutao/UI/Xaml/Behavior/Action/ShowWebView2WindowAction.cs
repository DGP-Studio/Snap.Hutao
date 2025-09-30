// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Microsoft.Xaml.Interactivity;
using Snap.Hutao.UI.Content;
using Snap.Hutao.UI.Xaml.View.Window.WebView2;
using WinRT;

namespace Snap.Hutao.UI.Xaml.Behavior.Action;

[DependencyProperty<IWebView2ContentProvider>("ContentProvider")]
internal sealed partial class ShowWebView2WindowAction : DependencyObject, IAction
{
    public static ShowWebView2WindowAction? TryShow<TProvider>(XamlRoot? xamlRoot)
        where TProvider : IWebView2ContentProvider, new()
    {
        try
        {
            return xamlRoot is null ? default : Show(new TProvider(), xamlRoot);
        }
        catch
        {
            return null;
        }
    }

    public static ShowWebView2WindowAction Show<TProvider>(XamlRoot xamlRoot)
        where TProvider : IWebView2ContentProvider, new()
    {
        return Show(new TProvider(), xamlRoot);
    }

    public static ShowWebView2WindowAction Show(IWebView2ContentProvider contentProvider, XamlRoot xamlRoot)
    {
        ShowWebView2WindowAction action = new()
        {
            ContentProvider = contentProvider,
        };

        action.ShowAt(xamlRoot);
        return action;
    }

    public object? Execute(object sender, object parameter)
    {
        ShowAt(sender.As<FrameworkElement>().XamlRoot);
        return default;
    }

    public void ShowAt(XamlRoot xamlRoot)
    {
        if (xamlRoot.XamlContext() is not { } xamlContext || ContentProvider is null)
        {
            return;
        }

        WebView2Window window = new(xamlContext.ServiceProvider, xamlRoot.ContentIslandEnvironment.AppWindowId, ContentProvider);
        window.Activate();
    }
}