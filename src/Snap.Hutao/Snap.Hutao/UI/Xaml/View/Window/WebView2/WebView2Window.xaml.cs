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
    private readonly WindowId parentWindowId;

    public WebView2Window(WindowId parentWindowId, IWebView2ContentProvider contentProvider)
    {
        windowScope = Ioc.Default.CreateScope();

        this.parentWindowId = parentWindowId;

        // Make sure this window has a parent window before we make modal
        SetWindowLongPtrW(this.GetWindowHandle(), WINDOW_LONG_PTR_INDEX.GWLP_HWNDPARENT, Win32Interop.GetWindowFromWindowId(parentWindowId));
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

    public FrameworkElement TitleBarCaptionAccess { get => TitleArea; }

    public IEnumerable<FrameworkElement> TitleBarPassthrough { get => []; }

    public new void Activate()
    {
        HWND parentHwnd = Win32Interop.GetWindowFromWindowId(parentWindowId);
        WindowExtension.SwitchTo(parentHwnd);
        EnableWindow(parentHwnd, false);
        base.Activate();

        double dpi = Math.Round(GetDpiForWindow(parentHwnd) / 96D, 2, MidpointRounding.AwayFromZero);
        AppWindow.MoveThenResize(contentProvider.InitializePosition(AppWindow.GetFromWindowId(parentWindowId).GetRect(), dpi));
    }

    public void OnWindowClosed()
    {
        HWND parentHwnd = Win32Interop.GetWindowFromWindowId(parentWindowId);
        EnableWindow(parentHwnd, true);

        // Reactive parent window
        SetForegroundWindow(parentHwnd);
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
            WebView.CoreWebView2.DocumentTitleChanged -= OnDocumentTitleChanged;
            WebView.CoreWebView2.HistoryChanged -= OnHistoryChanged;
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