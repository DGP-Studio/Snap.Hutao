// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
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

namespace Snap.Hutao.UI.Xaml.View.Window.WebView2;

[SuppressMessage("", "CA1001")]
[INotifyPropertyChanged]
internal sealed partial class WebView2Window : Microsoft.UI.Xaml.Window, IXamlWindowExtendContentIntoTitleBar, IXamlWindowClosedHandler
{
    private readonly CancellationTokenSource loadCts = new();

    private readonly IServiceScope windowScope;
    private readonly IWebView2ContentProvider contentProvider;
    private readonly AppWindow parentAppWindow;
    private readonly HWND parentHWND;

    public WebView2Window(WindowId parentWindowId, IWebView2ContentProvider contentProvider)
    {
        windowScope = Ioc.Default.CreateScope();

        parentHWND = Win32Interop.GetWindowFromWindowId(parentWindowId);
        parentAppWindow = AppWindow.GetFromWindowId(parentWindowId);

        // Make sure this window has a parent window before we make modal
        SetWindowLongPtrW(this.GetWindowHandle(), WINDOW_LONG_PTR_INDEX.GWLP_HWNDPARENT, parentHWND);
        if (AppWindow.Presenter is OverlappedPresenter presenter)
        {
            presenter.IsModal = true;
            presenter.IsResizable = false;
            presenter.IsMinimizable = false;
            presenter.IsMaximizable = false;
        }

        this.contentProvider = contentProvider;
        contentProvider.CloseWindowAction = Close;

        InitializeComponent();

        WebView.Loaded += OnWebViewLoaded;
        WebView.Unloaded += OnWebViewUnloaded;

        this.InitializeController(windowScope.ServiceProvider);
    }

    public FrameworkElement TitleBarAccess { get => TitleArea; }

    public new void Activate()
    {
        if (!IsWindowVisible(parentHWND))
        {
            ShowWindow(parentHWND, SHOW_WINDOW_CMD.SW_SHOW);
        }

        if (IsIconic(parentHWND))
        {
            ShowWindow(parentHWND, SHOW_WINDOW_CMD.SW_RESTORE);
        }

        SetForegroundWindow(parentHWND);

        EnableWindow(parentHWND, false);
        base.Activate();

        double dpi = Math.Round(GetDpiForWindow(parentHWND) / 96D, 2, MidpointRounding.AwayFromZero);
        AppWindow.MoveThenResize(contentProvider.InitializePosition(parentAppWindow.GetRect(), dpi));
    }

    public void OnWindowClosed()
    {
        EnableWindow(parentHWND, true);

        // Reactive parent window
        SetForegroundWindow(parentHWND);
        windowScope.Dispose();
    }

    [Command("GoBackCommand")]
    private void GoBack()
    {
        if (WebView.CoreWebView2.CanGoBack)
        {
            WebView.CoreWebView2.GoBack();
        }
    }

    [Command("RefreshCommand")]
    private void Refresh()
    {
        WebView.CoreWebView2.Reload();
    }

    private void OnWebViewLoaded(object sender, RoutedEventArgs e)
    {
        _ = OnWebViewLoadedAsync();

        [SuppressMessage("", "SH003")]
        async Task OnWebViewLoadedAsync()
        {
            await WebView.EnsureCoreWebView2Async();
            WebView.CoreWebView2.DocumentTitleChanged += OnDocumentTitleChanged;
            WebView.CoreWebView2.HistoryChanged += OnHistoryChanged;
            WebView.CoreWebView2.DisableDevToolsForReleaseBuild();
            contentProvider.CoreWebView2 = WebView.CoreWebView2;
            await contentProvider.InitializeAsync(windowScope.ServiceProvider, loadCts.Token).ConfigureAwait(false);
        }
    }

    private void OnWebViewUnloaded(object sender, RoutedEventArgs e)
    {
        loadCts.Cancel();
        loadCts.Dispose();
        contentProvider.Unload();

        if (WebView.CoreWebView2 is not null)
        {
            WebView.CoreWebView2.DocumentTitleChanged += OnDocumentTitleChanged;
            WebView.CoreWebView2.HistoryChanged += OnHistoryChanged;
        }

        WebView.Loaded -= OnWebViewLoaded;
        WebView.Unloaded -= OnWebViewUnloaded;
    }

    private void OnDocumentTitleChanged(CoreWebView2 sender, object args)
    {
        DocumentTitle.Text = sender.DocumentTitle;
    }

    private void OnHistoryChanged(CoreWebView2 sender, object args)
    {
        GoBackButton.IsEnabled = sender.CanGoBack;
    }

    private void OnActualThemeChanged(FrameworkElement sender, object args)
    {
        contentProvider.ActualTheme = sender.ActualTheme;
    }
}