// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.Web.WebView2.Core;
using Snap.Hutao.Core.Logging;
using Snap.Hutao.UI.Windowing;
using Snap.Hutao.UI.Windowing.Abstraction;
using Snap.Hutao.Web.WebView2;
using Snap.Hutao.Win32.Foundation;
using System.Collections.Immutable;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Snap.Hutao.UI.Xaml.View.Window.WebView2;

[SuppressMessage("", "CA1001")]
internal sealed partial class WebView2Window : Microsoft.UI.Xaml.Window,
    IXamlWindowExtendContentIntoTitleBar,
    IXamlWindowClosedHandler
{
    private readonly CancellationTokenSource loadCts = new();
    private readonly SemaphoreSlim webview2LoadLock = new(1, 1);

    private readonly IWebView2ContentProvider contentProvider;
    private readonly WindowId parentWindowId;
    private readonly IServiceScope scope;

    public WebView2Window(IServiceProvider serviceProvider, WindowId parentWindowId, IWebView2ContentProvider contentProvider)
    {
        this.parentWindowId = parentWindowId;

        try
        {
            // Make sure this window has a parent window before we make modal
            WindowUtilities.SetWindowOwner(this.GetWindowHandle(), Win32Interop.GetWindowFromWindowId(parentWindowId));
        }
        catch (FileLoadException)
        {
            // Ignore
        }

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

        scope = serviceProvider.CreateScope();
        this.InitializeController(scope.ServiceProvider);
    }

    public FrameworkElement TitleBarCaptionAccess { get => TitleArea; }

    public ImmutableArray<FrameworkElement> TitleBarPassthrough { get => []; }

    public new void Activate()
    {
        HWND parentHwnd = Win32Interop.GetWindowFromWindowId(parentWindowId);
        WindowUtilities.SwitchToWindow(parentHwnd);
        WindowUtilities.SetWindowIsEnabled(parentHwnd, false);
        base.Activate();

        AppWindow.MoveThenResize(contentProvider.InitializePosition(AppWindow.GetFromWindowId(parentWindowId).GetRect(), this.GetRasterizationScale()));
    }

    public void OnWindowClosing(out bool cancel)
    {
        if (webview2LoadLock.Wait(TimeSpan.Zero))
        {
            webview2LoadLock.Release();
            cancel = false;
            return;
        }

        cancel = true;
    }

    public void OnWindowClosed()
    {
        HWND parentHwnd = Win32Interop.GetWindowFromWindowId(parentWindowId);
        WindowUtilities.SetWindowIsEnabled(parentHwnd, true);
        WindowUtilities.SwitchToWindow(parentHwnd);

        using (webview2LoadLock)
        {
            webview2LoadLock.Wait();
            webview2LoadLock.Release();
        }
    }

    [Command("GoBackCommand")]
    private void GoBack()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Go back", "WebView2Window.Command"));

        if (WebView?.CoreWebView2 is null)
        {
            return;
        }

        if (WebView.CoreWebView2.CanGoBack)
        {
            WebView.CoreWebView2.GoBack();
        }
    }

    [Command("RefreshCommand")]
    private void Refresh()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Refresh", "WebView2Window.Command"));

        if (WebView?.CoreWebView2 is null)
        {
            return;
        }

        try
        {
            WebView.CoreWebView2.Reload();
        }
        catch (COMException)
        {
            // 组或资源的状态不是执行请求操作的正确状态
        }
    }

    private void OnWebViewLoaded(object sender, RoutedEventArgs e)
    {
        OnWebViewLoadedAsync().SafeForget();

        [SuppressMessage("", "SH003")]
        async Task OnWebViewLoadedAsync()
        {
            await webview2LoadLock.WaitAsync().ConfigureAwait(true);

            try
            {
                try
                {
                    CoreWebView2EnvironmentOptions options = new()
                    {
                        AdditionalBrowserArguments = "--do-not-de-elevate",
                    };
                    CoreWebView2Environment environment = await CoreWebView2Environment.CreateWithOptionsAsync(null, null, options);
                    await WebView.EnsureCoreWebView2Async(environment);
                }
                catch (SEHException)
                {
                    return;
                }

                // We observed that sometimes the CoreWebView2 is not ready even after EnsureCoreWebView2Async
                // System.NullReferenceException: Object reference not set to an instance of an object.
                if (!SpinWait.SpinUntil(() => WebView?.CoreWebView2 is not null, TimeSpan.FromSeconds(1)))
                {
                    WebView2LoadFailedHintText.Visibility = Visibility.Visible;
                    return;
                }

                WebView.CoreWebView2.DocumentTitleChanged += OnDocumentTitleChanged;
                WebView.CoreWebView2.HistoryChanged += OnHistoryChanged;
                WebView.CoreWebView2.DisableDevToolsForReleaseBuild();
                contentProvider.CoreWebView2 = WebView.CoreWebView2;
                await contentProvider.InitializeAsync(scope.ServiceProvider, loadCts.Token).ConfigureAwait(false);
            }
            finally
            {
                webview2LoadLock.Release();
            }
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