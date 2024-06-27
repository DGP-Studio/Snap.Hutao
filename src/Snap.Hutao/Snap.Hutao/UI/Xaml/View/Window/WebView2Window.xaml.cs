// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.Web.WebView2.Core;
using Snap.Hutao.UI.Windowing;
using Snap.Hutao.UI.Windowing.Abstraction;
using Snap.Hutao.Web.WebView2;
using Snap.Hutao.Win32.Foundation;
using Snap.Hutao.Win32.UI.WindowsAndMessaging;
using static Snap.Hutao.Win32.User32;

namespace Snap.Hutao.UI.Xaml.View.Window;

[SuppressMessage("", "CA1001")]
internal sealed partial class WebView2Window : Microsoft.UI.Xaml.Window, IXamlWindowExtendContentIntoTitleBar, IXamlWindowClosed
{
    private readonly CancellationTokenSource loadCts = new();
    private readonly IWebView2ContentProvider contentProvider;
    private readonly IServiceScope scope;
    private readonly AppWindow parentAppWindow;
    private readonly HWND parentHWND;

    public WebView2Window(WindowId parentWindowId, IWebView2ContentProvider contentProvider)
    {
        scope = Ioc.Default.CreateScope();

        parentHWND = Win32Interop.GetWindowFromWindowId(parentWindowId);
        parentAppWindow = AppWindow.GetFromWindowId(parentWindowId);

        SetWindowLongPtrW(this.GetWindowHandle(), WINDOW_LONG_PTR_INDEX.GWLP_HWNDPARENT, parentHWND);
        if (AppWindow.Presenter is OverlappedPresenter presenter)
        {
            presenter.IsModal = true;
        }

        this.contentProvider = contentProvider;
        InitializeComponent();

        WebView.Loaded += OnWebViewLoaded;
        WebView.Unloaded += OnWebViewUnloaded;

        this.InitializeController(scope.ServiceProvider);
    }

    public FrameworkElement TitleBarAccess { get => TitleArea; }

    public new void Activate()
    {
        EnableWindow(parentHWND, false);
        base.Activate();
        AppWindow.MoveAndResize(parentAppWindow.GetRect());
    }

    public void OnWindowClosed()
    {
        EnableWindow(parentHWND, true);
        SetForegroundWindow(parentHWND);
        scope.Dispose();
    }

    private void OnWebViewLoaded(object sender, RoutedEventArgs e)
    {
        OnWebViewLoadedAsync().SafeForget();

        async ValueTask OnWebViewLoadedAsync()
        {
            await WebView.EnsureCoreWebView2Async();
            WebView.CoreWebView2.DocumentTitleChanged += OnDocumentTitleChanged;
            WebView.CoreWebView2.DisableDevToolsForReleaseBuild();
            contentProvider.CoreWebView2 = WebView.CoreWebView2;
            await contentProvider.LoadAsync(loadCts.Token).ConfigureAwait(false);
        }
    }

    private void OnWebViewUnloaded(object sender, RoutedEventArgs e)
    {
        loadCts.Cancel();
        loadCts.Dispose();
        contentProvider.Unload();

        WebView.Loaded -= OnWebViewLoaded;
        WebView.Unloaded -= OnWebViewUnloaded;
    }

    private void OnDocumentTitleChanged(CoreWebView2 sender, object args)
    {
        DocumentTitle.Text = sender.DocumentTitle;
    }

    private void OnActualThemeChanged(FrameworkElement sender, object args)
    {
        contentProvider.ActualTheme = sender.ActualTheme;
    }
}