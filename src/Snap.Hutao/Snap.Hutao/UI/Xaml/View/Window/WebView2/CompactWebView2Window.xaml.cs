// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Input;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;
using Microsoft.Web.WebView2.Core;
using Snap.Hutao.Core.Setting;
using Snap.Hutao.UI.Input.LowLevel;
using Snap.Hutao.UI.Windowing.Abstraction;
using Snap.Hutao.UI.Xaml.Media.Animation;
using Snap.Hutao.Web.WebView2;
using Snap.Hutao.Win32.Foundation;
using Snap.Hutao.Win32.UI.Input.KeyboardAndMouse;
using Snap.Hutao.Win32.UI.WindowsAndMessaging;
using System.Globalization;
using System.Runtime.CompilerServices;
using Windows.Graphics;
using static Snap.Hutao.Win32.Macros;
using static Snap.Hutao.Win32.User32;

namespace Snap.Hutao.UI.Xaml.View.Window.WebView2;

[SuppressMessage("", "CA1001")]
internal sealed partial class CompactWebView2Window : Microsoft.UI.Xaml.Window,
    INotifyPropertyChanged,
    IXamlWindowExtendContentIntoTitleBar,
    IXamlWindowRectPersisted,
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

    private readonly CancellationTokenSource loadCts = new();
    private readonly Lock syncRoot = new();
    private readonly byte opacity;

    private readonly IServiceScope windowScope;
    private readonly ITaskContext taskContext;
    private readonly LowLevelKeyOptions lowLevelKeyOptions;
    private readonly InputPointerSource inputPointerSource;
    private readonly InputNonClientPointerSource inputNonClientPointerSource;

    private bool isLocked;

    public CompactWebView2Window()
    {
        windowScope = Ioc.Default.CreateScope();
        taskContext = windowScope.ServiceProvider.GetRequiredService<ITaskContext>();
        lowLevelKeyOptions = windowScope.ServiceProvider.GetRequiredService<LowLevelKeyOptions>();

        opacity = (byte)(LocalSetting.Get(SettingKeys.CompactWebView2WindowInactiveOpacity, 50D) * 255 / 100);

        if (AppWindow.Presenter is OverlappedPresenter presenter)
        {
            presenter.SetBorderAndTitleBar(true, false);
            presenter.IsAlwaysOnTop = true;
        }

        InitializeComponent();

        TitleBarPassthrough = [SourceTextBox];

        RootGrid.DataContext = this;

        inputPointerSource = this.GetInputPointerSource();
        inputPointerSource.PointerEntered += OnWindowPointerEntered;
        inputPointerSource.PointerExited += OnWindowPointerExited;

        inputNonClientPointerSource = this.GetInputNonClientPointerSource();
        inputNonClientPointerSource.PointerEntered += OnWindowNonClientPointerEntered;
        inputNonClientPointerSource.PointerExited += OnWindowNonClientPointerExited;

        WebView.Loaded += OnWebViewLoaded;
        WebView.Unloaded += OnWebViewUnloaded;
        WebView.NavigationStarting += OnWebViewNavigationStarting;
        WebView.NavigationCompleted += OnWebViewNavigationCompleted;

        this.InitializeController(windowScope.ServiceProvider);

        UpdateLayeredWindow();
        InputLowLevelKeyboardSource.Initialize();
        InputLowLevelKeyboardSource.KeyDown += OnLowLevelKeyDown;
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
                        WebView.Source = uri;
                        OnPropertyChanged();
                    }
                }
            }
        }
    }

    public IEnumerable<FrameworkElement> TitleBarPassthrough { get; }

    public string PersistRectKey { get => SettingKeys.CompactWebView2WindowRect; }

    public string PersistScaleKey { get => SettingKeys.CompactWebView2WindowScale; }

    public SizeInt32 InitSize { get => new(800, 600); }

    public SizeInt32 MinSize { get => new(200, 200); }

    public void OnWindowClosed()
    {
        LocalSetting.Set(SettingKeys.CompactWebView2WindowPreviousSourceUrl, Source);

        inputPointerSource.PointerEntered -= OnWindowPointerEntered;
        inputPointerSource.PointerExited -= OnWindowPointerExited;
        inputNonClientPointerSource.PointerEntered -= OnWindowNonClientPointerEntered;
        inputNonClientPointerSource.PointerExited -= OnWindowNonClientPointerExited;
        windowScope.Dispose();
    }

    private static void OnDownloadStarting(CoreWebView2 sender, CoreWebView2DownloadStartingEventArgs args)
    {
        args.Cancel = true;
    }

    private static void OnNewWindowRequested(object? sender, CoreWebView2NewWindowRequestedEventArgs args)
    {
        args.Handled = true;
        ArgumentNullException.ThrowIfNull(sender);
        ((CoreWebView2)sender).Navigate(args.Uri);
    }

    private void OnWindowPointerEntered(InputPointerSource source, PointerEventArgs args)
    {
        UpdateLayeredWindow();
    }

    private void OnWindowNonClientPointerEntered(InputNonClientPointerSource source, NonClientPointerEventArgs args)
    {
        UpdateLayeredWindow();
    }

    private void OnWindowPointerExited(InputPointerSource source, PointerEventArgs args)
    {
        UpdateLayeredWindow();
    }

    private void OnWindowNonClientPointerExited(InputNonClientPointerSource source, NonClientPointerEventArgs args)
    {
        UpdateLayeredWindow();
    }

    private void UpdateLayeredWindow()
    {
        if (opacity >= 255)
        {
            return;
        }

        lock (syncRoot)
        {
            if (GetCursorPos(out POINT pt) && GetWindowRect(this.GetWindowHandle(), out RECT rect) && PtInRect(in rect, pt))
            {
                this.RemoveExStyleLayered();
            }
            else
            {
                this.AddExStyleLayered();
                SetLayeredWindowAttributes(this.GetWindowHandle(), RGB(0, 0, 0), opacity, LAYERED_WINDOW_ATTRIBUTES_FLAGS.LWA_COLORKEY | LAYERED_WINDOW_ATTRIBUTES_FLAGS.LWA_ALPHA);
            }
        }
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

    [Command("StopRefreshCommand")]
    private void StopRefresh()
    {
        WebView.CoreWebView2.Stop();
    }

    [Command("ToggleLockCommand")]
    private void ToggleLock()
    {
        isLocked = !isLocked;
        ToggleLockButton.Content = isLocked ? "\uE72E" : "\uE785";
        TitleBarRowDefinition.Height = isLocked ? Constants.ZeroGridLength : GridLength.Auto;
        this.GetController()?.UpdateDragRectangles();
    }

    [Command("CloseCommand")]
    private void CloseWindow()
    {
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

        if (key == lowLevelKeyOptions.WebView2VideoPlayPauseKey.Value)
        {
            taskContext.BeginInvokeOnMainThread(() =>
            _ = WebView.CoreWebView2.ExecuteScriptAsync(VideoPlayPauseScript));
            return;
        }

        if (key == lowLevelKeyOptions.WebView2VideoFastForwardKey.Value)
        {
            int seconds = LocalSetting.Get(SettingKeys.WebView2VideoFastForwardOrRewindSeconds, 5);
            taskContext.BeginInvokeOnMainThread(() =>
            _ = WebView.CoreWebView2.ExecuteScriptAsync(string.Format(CultureInfo.CurrentCulture, VideoFastForwardScript, seconds)));
            return;
        }

        if (key == lowLevelKeyOptions.WebView2VideoRewindKey.Value)
        {
            int seconds = LocalSetting.Get(SettingKeys.WebView2VideoFastForwardOrRewindSeconds, 5);
            taskContext.BeginInvokeOnMainThread(() =>
            _ = WebView.CoreWebView2.ExecuteScriptAsync(string.Format(CultureInfo.CurrentCulture, VideoRewindScript, seconds)));
            return;
        }
    }

    private void OnWebViewLoaded(object sender, RoutedEventArgs e)
    {
        _ = OnWebViewLoadedAsync();

        [SuppressMessage("", "SH003")]
        async Task OnWebViewLoadedAsync()
        {
            await WebView.EnsureCoreWebView2Async();
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

    private void OnWebViewUnloaded(object sender, RoutedEventArgs e)
    {
        InputLowLevelKeyboardSource.KeyDown -= OnLowLevelKeyDown;
        InputLowLevelKeyboardSource.Uninitialize();

        loadCts.Cancel();
        loadCts.Dispose();

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
        this.GetController()?.UpdateDragRectangles();
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