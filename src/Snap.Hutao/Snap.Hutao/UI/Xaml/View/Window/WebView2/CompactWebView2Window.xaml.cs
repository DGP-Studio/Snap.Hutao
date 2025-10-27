// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Input;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;
using Microsoft.Web.WebView2.Core;
using Snap.Hutao.Core.Logging;
using Snap.Hutao.Core.Setting;
using Snap.Hutao.UI.Input.LowLevel;
using Snap.Hutao.UI.Windowing;
using Snap.Hutao.UI.Windowing.Abstraction;
using Snap.Hutao.UI.Xaml.Media.Animation;
using Snap.Hutao.Web.WebView2;
using Snap.Hutao.Win32.Foundation;
using Snap.Hutao.Win32.UI.Input.KeyboardAndMouse;
using System.Collections.Immutable;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Windows.Graphics;
using WinRT;

namespace Snap.Hutao.UI.Xaml.View.Window.WebView2;

[SuppressMessage("", "CA1001")]
[Service(ServiceLifetime.Transient)]
internal sealed partial class CompactWebView2Window : Microsoft.UI.Xaml.Window,
    INotifyPropertyChanged,
    IXamlWindowExtendContentIntoTitleBar,
    IXamlWindowHasInitSize,
    IXamlWindowClosedHandler
{
    /*lang=javascript*/
    private const string VideoPlayPauseScript = """
        {
            let v = document.evaluate("//video", document, null).iterateNext();
            v && (v.paused ? v.play() : v.pause());
        }
        """;

    /*lang=javascript*/
    private const string VideoFastForwardScript = """
        {{
            let v = document.evaluate("//video", document, null).iterateNext();
            v && (v.currentTime += {0})
        }}
        """;

    /*lang=javascript*/
    private const string VideoRewindScript = """
        {{
            let v = document.evaluate("//video", document, null).iterateNext();
            v && (v.currentTime -= {0})
        }}
        """;

    private readonly CancellationTokenSource webview2LoadCts = new();
    private readonly SemaphoreSlim webview2LoadLock = new(1, 1);
    private readonly Lock layeredWindowLock = new();
    private readonly byte opacity;

    private readonly LowLevelKeyOptions lowLevelKeyOptions;
    private readonly ITaskContext taskContext;

    private bool isLocked;

    public CompactWebView2Window(IServiceProvider serviceProvider)
    {
        opacity = (byte)(LocalSetting.Get(SettingKeys.CompactWebView2WindowInactiveOpacity, 50D) * 255 / 100);

        if (AppWindow.Presenter is OverlappedPresenter presenter)
        {
            presenter.SetBorderAndTitleBar(true, false);
            presenter.IsAlwaysOnTop = true;
        }

        InitializeComponent();

        TitleBarPassthrough = [SourceTextBox];

        RootGrid.DataContext = this;

        WebView.Loaded += OnWebViewLoaded;
        WebView.Unloaded += OnWebViewUnloaded;
        WebView.NavigationStarting += OnWebViewNavigationStarting;
        WebView.NavigationCompleted += OnWebViewNavigationCompleted;

        IServiceScope scope = serviceProvider.CreateScope();
        this.InitializeController(scope.ServiceProvider);
        taskContext = scope.ServiceProvider.GetRequiredService<ITaskContext>();
        lowLevelKeyOptions = scope.ServiceProvider.GetRequiredService<LowLevelKeyOptions>();

        InputActivationListener.GetForWindowId(AppWindow.Id).InputActivationChanged += OnInputActivationChanged;

        InputLowLevelKeyboardSource.KeyDown += OnLowLevelKeyDown;
        InputLowLevelKeyboardSource.Initialize();
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public FrameworkElement TitleBarCaptionAccess { get => TitleArea; }

    public string Source
    {
        get => WebView.Source?.ToString() ?? string.Empty;
        set
        {
            if (!string.IsNullOrEmpty(value))
            {
                string url = value.StartsWith("https://", StringComparison.OrdinalIgnoreCase) || value.StartsWith("http://", StringComparison.OrdinalIgnoreCase)
                    ? value
                    : $"https://{value}";

                if (Uri.TryCreate(url, UriKind.Absolute, out Uri? uri))
                {
                    if (!EqualityComparer<Uri>.Default.Equals(WebView.Source, uri))
                    {
                        try
                        {
                            WebView.Source = uri;
                            OnPropertyChanged();
                        }
                        catch (COMException)
                        {
                            // 不是有效的绝对 URI
                        }
                    }
                }
            }
        }
    }

    public ImmutableArray<FrameworkElement> TitleBarPassthrough { get; }

    public SizeInt32 InitSize { get => ScaledSizeInt32.CreateForWindow(800, 600, this); }

    public void OnWindowClosing(out bool cancel)
    {
        // WebView2 is still loading
        if (!webview2LoadLock.Wait(TimeSpan.Zero))
        {
            cancel = true;
            return;
        }

        cancel = false;

        LocalSetting.Set(SettingKeys.CompactWebView2WindowPreviousSourceUrl, Source);

        InputLowLevelKeyboardSource.KeyDown -= OnLowLevelKeyDown;
        InputLowLevelKeyboardSource.Uninitialize();

        InputActivationListener.GetForWindowId(AppWindow.Id).InputActivationChanged -= OnInputActivationChanged;
        webview2LoadLock.Release();
    }

    public void OnWindowClosed()
    {
        using (webview2LoadLock)
        {
            webview2LoadLock.Wait();
            webview2LoadLock.Release();
        }
    }

    private static void OnDownloadStarting(CoreWebView2 sender, CoreWebView2DownloadStartingEventArgs args)
    {
        args.Cancel = true;
    }

    private static void OnNewWindowRequested(object? sender, CoreWebView2NewWindowRequestedEventArgs args)
    {
        args.Handled = true;
        ArgumentNullException.ThrowIfNull(sender);
        sender.As<CoreWebView2>().Navigate(args.Uri);
    }

    private void OnInputActivationChanged(InputActivationListener sender, InputActivationListenerActivationChangedEventArgs args)
    {
        InputActivationState state = sender.State;
        UpdateLayeredWindow(state is InputActivationState.Activated);
    }

    private void UpdateLayeredWindow(bool enter)
    {
        if (opacity >= 255)
        {
            return;
        }

        lock (layeredWindowLock)
        {
            HWND windowHandle = this.GetWindowHandle();
            if (windowHandle.Value is 0)
            {
                return;
            }

            try
            {
                if (enter)
                {
                    this.RemoveExtendedStyleLayered();
                }
                else
                {
                    this.AddExtendedStyleLayered();
                    this.SetLayeredWindowTransparency(opacity);
                }
            }
            catch
            {
                // Ignore
            }
        }
    }

    [Command("GoBackCommand")]
    private void GoBack()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Go back", "CompactWebView2Window.Command"));

        if (WebView?.CoreWebView2 is { CanGoBack: true })
        {
            WebView.CoreWebView2.GoBack();
        }
    }

    [Command("RefreshCommand")]
    private void Refresh()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Refresh", "CompactWebView2Window.Command"));
        WebView?.CoreWebView2?.Reload();
    }

    [Command("StopRefreshCommand")]
    private void StopRefresh()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Stop refresh", "CompactWebView2Window.Command"));
        WebView.CoreWebView2.Stop();
    }

    [Command("ToggleLockCommand")]
    private void ToggleLock()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Toggle lock", "CompactWebView2Window.Command"));

        isLocked = !isLocked;
        ToggleLockButton.Content = isLocked ? "\uE72E" : "\uE785";
        TitleBarRowDefinition.Height = isLocked ? Constants.ZeroGridLength : GridLength.Auto;
        XamlWindowRegionRects.Update(this);
    }

    [Command("CloseCommand")]
    private void CloseWindow()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Close window", "CompactWebView2Window.Command"));
        Close();
    }

    private void OnLowLevelKeyDown(LowLevelKeyEventArgs args)
    {
        if (args.Handled)
        {
            return;
        }

        VIRTUAL_KEY key = (VIRTUAL_KEY)args.Data.vkCode;
        if (key is VIRTUAL_KEY.VK__none_)
        {
            // Skipping VK__none_ handling
            return;
        }

        CoreWebView2 coreWebView2 = WebView.CoreWebView2;

        if (coreWebView2 is null)
        {
            return;
        }

        if (key == lowLevelKeyOptions.WebView2VideoPlayPauseKey.Value)
        {
            _ = taskContext.InvokeOnMainThread(() => coreWebView2.ExecuteScriptAsync(VideoPlayPauseScript));
            return;
        }

        if (key == lowLevelKeyOptions.WebView2VideoFastForwardKey.Value)
        {
            int seconds = LocalSetting.Get(SettingKeys.WebView2VideoFastForwardOrRewindSeconds, 5);
            _ = taskContext.InvokeOnMainThread(() => coreWebView2.ExecuteScriptAsync(string.Format(CultureInfo.CurrentCulture, VideoFastForwardScript, seconds)));
            return;
        }

        if (key == lowLevelKeyOptions.WebView2VideoRewindKey.Value)
        {
            int seconds = LocalSetting.Get(SettingKeys.WebView2VideoFastForwardOrRewindSeconds, 5);
            _ = taskContext.InvokeOnMainThread(() => coreWebView2.ExecuteScriptAsync(string.Format(CultureInfo.CurrentCulture, VideoRewindScript, seconds)));
            return;
        }

        if (key == lowLevelKeyOptions.WebView2HideKey.Value)
        {
            AppWindow appWindow = AppWindow;
            taskContext.InvokeOnMainThread(() =>
            {
                if (appWindow.IsVisible)
                {
                    appWindow.Hide();
                }
                else
                {
                    appWindow.Show(false);
                    appWindow.MoveInZOrderAtTop();
                }
            });
            return;
        }

        GC.KeepAlive(coreWebView2);
    }

    private void OnWebViewLoaded(object sender, RoutedEventArgs e)
    {
        OnWebViewLoadedAsync().SafeForget();

        [SuppressMessage("", "SH003")]
        async Task OnWebViewLoadedAsync()
        {
            using (await webview2LoadLock.EnterAsync().ConfigureAwait(true))
            {
                try
                {
                    CoreWebView2EnvironmentOptions options = new()
                    {
                        AdditionalBrowserArguments = "--do-not-de-elevate --autoplay-policy=no-user-gesture-required",
                    };
                    CoreWebView2Environment environment = await CoreWebView2Environment.CreateWithOptionsAsync(null, null, options);
                    await WebView.EnsureCoreWebView2Async(environment);
                }
                catch (SEHException ex)
                {
                    SentrySdk.CaptureException(ex);
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
                WebView.CoreWebView2.DownloadStarting += OnDownloadStarting;
                WebView.CoreWebView2.SourceChanged += OnSourceChanged;
                WebView.CoreWebView2.HistoryChanged += OnHistoryChanged;
                WebView.CoreWebView2.NewWindowRequested += OnNewWindowRequested;
                WebView.CoreWebView2.DisableDevToolsForReleaseBuild();

                await taskContext.SwitchToMainThreadAsync();
                Source = LocalSetting.Get(SettingKeys.CompactWebView2WindowPreviousSourceUrl, string.Empty);
            }
        }
    }

    private void OnWebViewUnloaded(object sender, RoutedEventArgs e)
    {
        webview2LoadCts.Cancel();
        webview2LoadCts.Dispose();

        if (WebView.CoreWebView2 is not null)
        {
            WebView.CoreWebView2.DocumentTitleChanged -= OnDocumentTitleChanged;
            WebView.CoreWebView2.SourceChanged -= OnSourceChanged;
            WebView.CoreWebView2.DownloadStarting -= OnDownloadStarting;
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

    private void OnSourceChanged(CoreWebView2 sender, CoreWebView2SourceChangedEventArgs args)
    {
        OnPropertyChanged(nameof(Source));
    }

    private void OnSourceTextBoxKeyDown(object sender, KeyRoutedEventArgs args)
    {
        if (args.Key is Windows.System.VirtualKey.Enter)
        {
            WebView.Focus(FocusState.Programmatic);
        }
    }

    private void OnDocumentTitleSizeChanged(object sender, SizeChangedEventArgs e)
    {
        XamlWindowRegionRects.Update(this);
    }

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new(propertyName));
    }

    private void OnWebViewNavigationStarting(object sender, CoreWebView2NavigationStartingEventArgs e)
    {
        RefreshButton.Content = "\uF78A";
        RefreshButton.Command = StopRefreshCommand;
        ProgressRing.Visibility = Visibility.Visible;
    }

    private void OnWebViewNavigationCompleted(object sender, CoreWebView2NavigationCompletedEventArgs e)
    {
        RefreshButton.Content = "\uE72C";
        RefreshButton.Command = RefreshCommand;
        ProgressRing.Visibility = Visibility.Collapsed;
    }
}